using System.Collections.Concurrent;

namespace DIToolSystem.DICustomContainer;

public class Container : IContainer
{
    private readonly ConcurrentDictionary<Type, Registration> _registrations = new();

    public IContainer Register<TInterface, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient) 
        where TInterface : class where TImplementation : class, TInterface
    {
        var registration = new Registration(typeof(TInterface), typeof(TImplementation), lifetime);
        _registrations.AddOrUpdate(typeof(TInterface), registration, (_, _) => registration);
        return this;
    }

    public IContainer Register<TInterface>(Func<IContainer, TInterface> factory, ServiceLifetime lifetime = ServiceLifetime.Transient) 
        where TInterface : class
    {
        var registration = new Registration(typeof(TInterface), factory, lifetime);
        _registrations.AddOrUpdate(typeof(TInterface), registration, (_, _) => registration);
        return this;
    }

    public IContainer Register<TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Transient) 
        where TImplementation : class
    {
        var registration = new Registration(typeof(TImplementation), typeof(TImplementation), lifetime);
        _registrations.AddOrUpdate(typeof(TImplementation), registration, (_, _) => registration);
        return this;
    }
}