using System.Collections.Concurrent;

namespace DIToolSystem.DICustomContainer;

public class Container : IContainer
{
    private KeyValuePair<RegistrationIndexer, RegistrationInfo> _lastRegistration;
    private readonly ConcurrentDictionary<RegistrationIndexer, RegistrationInfo> _registrations = new();
    private readonly ConcurrentDictionary<Type, Type> _resolutions = new();

    public IContainer Register<TImplementation, TInterface>()
        where TInterface : class where TImplementation : class, TInterface
    {
        var registrationInfo = new RegistrationInfo(typeof(TImplementation), typeof(TInterface));
        var registrationIndexer = new RegistrationIndexer(typeof(TInterface));
        AddRegistration(registrationInfo, registrationIndexer);
        return this;
    }

    public IContainer Register<TService>(Func<IContainer, TService> factory)
        where TService : class
    {
        var registrationInfo = new RegistrationInfo(typeof(TService), factory);
        var registrationIndexer = new RegistrationIndexer(typeof(TService));
        AddRegistration(registrationInfo, registrationIndexer);
        return this;
    }

    public IContainer Register<TImplementation>()
        where TImplementation : class
    {
        var registrationInfo = new RegistrationInfo(typeof(TImplementation), typeof(TImplementation));
        var registrationIndexer = new RegistrationIndexer(typeof(TImplementation));
        AddRegistration(registrationInfo, registrationIndexer);
        return this;
    }

    private void AddRegistration(RegistrationInfo registrationInfo, RegistrationIndexer registrationIndexer)
    {
        registrationInfo.Lifetime = ServiceLifetime.Transient;
        _registrations[registrationIndexer] = registrationInfo;
        _lastRegistration =
            new KeyValuePair<RegistrationIndexer, RegistrationInfo>(registrationIndexer, registrationInfo);
    }

    public IContainer SingleInstance()
    {
        if (_lastRegistration.Value == null)
            throw new InvalidOperationException("No registration available to set as single instance.");

        _lastRegistration.Value.Lifetime = ServiceLifetime.Singleton;
        return this;
    }

    public IContainer Named(string name)
    {
        if (_lastRegistration.Value == null)
            throw new InvalidOperationException("No registration available to name.");

        _registrations.TryRemove(_lastRegistration);
        var indexer = new RegistrationIndexer(_lastRegistration.Key.ServiceType) { Name = name };
        _registrations[indexer] = _lastRegistration.Value;
        _lastRegistration = new KeyValuePair<RegistrationIndexer, RegistrationInfo>(indexer, _lastRegistration.Value);
        return this;
    }

    public TService Resolve<TService>() where TService : class =>
        (TService)Resolve(new RegistrationIndexer(typeof(TService)));

    public TService ResolveNamed<TService>(string name) where TService : class =>
        (TService)Resolve(new RegistrationIndexer(typeof(TService)) { Name = name });

    public IEnumerable<TService> ResolveAll<TService>() where TService : class
    {
        throw new NotImplementedException();
    }

    public Task<TService> ResolveAsync<TService>() where TService : class => Task.FromResult(Resolve<TService>());

    private RegistrationInfo? GetRegistrationInfo(RegistrationIndexer registrationIndexer)
    {
        var matches =
            _registrations.Where(kvp => kvp.Key.CompareTo(registrationIndexer) == 0).Select(kvp => kvp.Value).ToList();

        return matches.Count switch
        {
            0 => null,
            1 => matches[0],
            _ => throw new InvalidOperationException(
                $"Multiple registrations found for type {registrationIndexer.ServiceType}.")
        };
    }

    private object Resolve(RegistrationIndexer indexer)
    {
        var type = indexer.ServiceType;

        // Handle lazy resolution
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>))
            return ResolveLazy(type);

        // Track resolutions
        if (!_resolutions.TryAdd(type, type))
            throw new InvalidOperationException($"Circular dependency detected for {type}");

        try
        {
            // If the type is registered, resolve it according to its registration
            var registration = GetRegistrationInfo(indexer);
            if (registration != null)
                return ResolveInstance(registration);

            // If not registered, attempt to initialize the type directly if it's concrete
            return type is { IsInterface: true } or { IsAbstract: true }
                ? throw new InvalidOperationException($"Cannot resolve unregistered type: {type}")
                : InitializeInstance(type);
        }
        finally
        {
            // Remove from resolution tracking once resolved
            _resolutions.TryRemove(type, out _);
        }
    }

    private object ResolveLazy(Type type)
    {
        var innerType = type.GetGenericArguments().FirstOrDefault()
                        ?? throw new InvalidOperationException($"Lazy type {type} must have a generic argument.");

        var lazyConstructor = type.GetConstructor([typeof(Func<>).MakeGenericType(innerType)])
                              ?? throw new InvalidOperationException($"No suitable constructor found for {type}.");

        var resolveGeneric = GetType()
            .GetMethod(nameof(Resolve), Type.EmptyTypes)!
            .GetGenericMethodDefinition()
            .MakeGenericMethod(innerType);
        var factoryType = typeof(Func<>).MakeGenericType(innerType);
        var factory = Delegate.CreateDelegate(factoryType, this, resolveGeneric);

        return lazyConstructor.Invoke([factory]);
    }

    private object ResolveInstance(RegistrationInfo registrationInfo) =>
        registrationInfo.Lifetime switch
        {
            ServiceLifetime.Singleton => registrationInfo.SingleInstance ??= InitializeInstance(registrationInfo),
            _ => InitializeInstance(registrationInfo)
        };

    private object InitializeInstance(RegistrationInfo registrationInfo) =>
        registrationInfo switch
        {
            { Factory: not null } => registrationInfo.Factory(this),
            { ImplementationType: not null } => InitializeInstance(registrationInfo.ImplementationType),
            _ => throw new InvalidOperationException("Invalid registration: no factory or implementation type.")
        };

    private object InitializeInstance(Type type)
    {
        var constructor = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault()
                          ?? throw new InvalidOperationException($"No public constructors found for {type}");

        var parameterInstances = constructor.GetParameters()
            .Select(p => Resolve(new RegistrationIndexer(p.ParameterType))).ToArray();
        return constructor.Invoke(parameterInstances);
    }
}