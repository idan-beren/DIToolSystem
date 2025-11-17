namespace DIToolSystem.DICustomContainer;

public class RegistrationInfo
{
    public Type ServiceType { get; }
    
    public Type? ImplementationType { get; }
    
    public Func<IContainer, object>? Factory { get; }
    
    public ServiceLifetime? Lifetime { get; set; }
    
    public object? SingleInstance { get; set;  }
    
    public RegistrationInfo(Type implementationType, Type interfaceType)
    {
        ServiceType = interfaceType;
        ImplementationType = implementationType;
        Factory = null;
        Lifetime = null;
        SingleInstance = null;
    }
    
    public RegistrationInfo(Type serviceType, Func<IContainer, object> factory)
    {
        ServiceType = serviceType;
        ImplementationType = null;
        Factory = factory;
        Lifetime = null;
        SingleInstance = null;
    }
}