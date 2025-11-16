namespace DIToolSystem.DICustomContainer;

public class Registration
{
    public Type ServiceType { get; }
    
    public Type? ImplementationType { get; }
    
    public Func<IContainer, object>? Factory { get; }
    
    public ServiceLifetime? Lifetime { get; set; }
    
    public object? SingleInstance { get; set;  }
    
    public string? Name { get; set; }

    public Registration(Type serviceType, Type implementationType)
    {
        ServiceType = serviceType;
        ImplementationType = implementationType;
        Factory = null;
        Lifetime = null;
        SingleInstance = null;
        Name = null;
    }
    
    public Registration(Type serviceType, Func<IContainer, object> factory)
    {
        ServiceType = serviceType;
        ImplementationType = null;
        Factory = factory;
        Lifetime = null;
        SingleInstance = null;
        Name = null;
    }
}