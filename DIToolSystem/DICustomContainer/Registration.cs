namespace DIToolSystem.DICustomContainer;

public class Registration
{
    public Type ServiceType { get; }
    
    public Type? ImplementationType { get; }
    
    public Func<IContainer, object>? Factory { get; }
    
    public ServiceLifetime Lifetime { get; }
    
    public object? Instance { get; set;  }

    public Registration(Type serviceType, Type implementationType, ServiceLifetime lifetime)
    {
        ServiceType = serviceType;
        ImplementationType = implementationType;
        Factory = null;
        Lifetime = lifetime;
        Instance = null;
    }
    
    public Registration(Type serviceType, Func<IContainer, object> factory, ServiceLifetime lifetime)
    {
        ServiceType = serviceType;
        ImplementationType = null;
        Factory = factory;
        Lifetime = lifetime;
        Instance = null;
    }
}