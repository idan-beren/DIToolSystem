namespace DIToolSystem.DICustomContainer;

public interface IContainer
{
    public IContainer Register<TInterface, TImplementation>(ServiceLifetime lifetime)
        where TInterface : class 
        where TImplementation : class, TInterface;
    
    public IContainer Register<TInterface>(Func<IContainer, TInterface> factory, ServiceLifetime lifetime) 
        where TInterface : class;
    
    public IContainer Register<TImplementation>(ServiceLifetime lifetime)
        where TImplementation : class;
}