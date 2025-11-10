using System.Collections.Concurrent;

namespace DIToolSystem.DICustomContainer;

public class Container : IContainer
{
    private readonly ConcurrentDictionary<Type, Registration> _registrations = new();
    private readonly ConcurrentDictionary<Type, Type> _resolutions = new();

    public IContainer Register<TInterface, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TInterface : class where TImplementation : class, TInterface
    {
        _registrations[typeof(TInterface)] = new Registration(typeof(TInterface), typeof(TImplementation), lifetime);
        return this;
    }

    public IContainer Register<TInterface>(Func<IContainer, TInterface> factory,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TInterface : class
    {
        _registrations[typeof(TInterface)] = new Registration(typeof(TInterface), factory, lifetime);
        return this;
    }

    public IContainer Register<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TImplementation : class
    {
        _registrations[typeof(TImplementation)] =
            new Registration(typeof(TImplementation), typeof(TImplementation), lifetime);
        return this;
    }

    public T Resolve<T>() where T : class => (T)Resolve(typeof(T));

    public object Resolve(Type type)
    {
        // Handle Lazy<T> resolution
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>))
            return ResolveLazy(type);

        // Track resolutions
        if (!_resolutions.TryAdd(type, type))
            throw new InvalidOperationException($"Circular dependency detected for {type}");

        try
        {
            // If the type is registered, resolve it according to its registration
            if (_registrations.TryGetValue(type, out var registration))
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

    private object ResolveInstance(Registration registration) =>
        registration.Lifetime switch
        {
            ServiceLifetime.Singleton => registration.Instance ??= InitializeInstance(registration),
            _ => InitializeInstance(registration)
        };

    private object InitializeInstance(Registration registration) =>
        registration switch
        {
            { Factory: not null } => registration.Factory(this),
            { ImplementationType: not null } => InitializeInstance(registration.ImplementationType),
            _ => throw new InvalidOperationException("Invalid registration: no factory or implementation type.")
        };

    private object InitializeInstance(Type type)
    {
        var constructor = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault()
                          ?? throw new InvalidOperationException($"No public constructors found for {type}");

        var parameterInstances = constructor.GetParameters().Select(p => Resolve(p.ParameterType)).ToArray();
        return constructor.Invoke(parameterInstances);
    }
}