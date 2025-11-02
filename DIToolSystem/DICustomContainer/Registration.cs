namespace DIToolSystem.DICustomContainer;

public class Registration
{
    public Type InterfaceType { get; }
    
    public Type? ImplementationType { get; }
    
    public Func<IContainer, object>? Factory { get; }

    public Registration(Type interfaceType, Type implementationType)
    {
        InterfaceType = interfaceType;
        ImplementationType = implementationType;
        Factory = null;
    }
    
    public Registration(Type interfaceType, Func<IContainer, object> factory)
    {
        InterfaceType = interfaceType;
        ImplementationType = null;
        Factory = factory;
    }
}