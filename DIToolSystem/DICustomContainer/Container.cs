using System.Collections.Concurrent;

namespace DIToolSystem.DICustomContainer;

public class Container : IContainer
{
    private readonly ConcurrentDictionary<Type, Registration> _registrations = new();

    public IContainer Register<TInterface, TImplementation>() where TInterface : class where TImplementation : class, TInterface
    {
        var registration = new Registration(typeof(TInterface), typeof(TImplementation));
        _registrations.AddOrUpdate(typeof(TInterface), registration, (_, _) => registration);
        return this;
    }

    public IContainer Register<TInterface>(Func<IContainer, TInterface> factory) where TInterface : class
    {
        var registration = new Registration(typeof(TInterface), factory);
        _registrations.AddOrUpdate(typeof(TInterface), registration, (_, _) => registration);
        return this;
    }

    public IContainer Register<TImplementation>() where TImplementation : class
    {
        var registration = new Registration(typeof(TImplementation), typeof(TImplementation));
        _registrations.AddOrUpdate(typeof(TImplementation), registration, (_, _) => registration);
        return this;
    }
}