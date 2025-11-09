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

    public IContainer Register<TInterface>(Func<IContainer, TInterface> factory, ServiceLifetime lifetime = ServiceLifetime.Transient)
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

    public TType Resolve<TType>() where TType : class => (TType)Resolve(typeof(TType));

    public object Resolve(Type type)
    {
        if (!_resolutions.TryAdd(type, type))
            throw new InvalidOperationException($"Circular dependency detected for {type}");

        try
        {
            if (_registrations.TryGetValue(type, out var registration))
                return ResolveInstance(registration);

            if (type is { IsInterface: false, IsAbstract: false })
                return CreateInstance(type);

            throw new InvalidOperationException($"Cannot resolve unregistered type: {type}");
        }
        finally
        {
            _resolutions.TryRemove(type, out _);
        }
    }
    
    private object ResolveInstance(Registration registration)
    {
        if (registration.Lifetime == ServiceLifetime.Singleton)
            if (registration.Instance is not null)
                return registration.Instance;
            else
            {
                var instance = CreateInstance(registration);
                registration.Instance = instance;
                return instance;
            }
        
        return CreateInstance(registration);
    }
    
    private object CreateInstance(Registration registration)
    {
        if (registration.Factory is not null)
            return registration.Factory(this);

        if (registration.ImplementationType is not null)
            return CreateInstance(registration.ImplementationType);

        throw new InvalidOperationException("Invalid registration: no factory or implementation type.");
    }
    
    private object CreateInstance(Type type)
    {
        var constructor = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
        if (constructor is null)
            throw new InvalidOperationException($"No public constructors found for {type}");

        var parameters = constructor.GetParameters();
        var parameterInstances = new object[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
            parameterInstances[i] = Resolve(parameters[i].ParameterType);

        return constructor.Invoke(parameterInstances);
    }
}