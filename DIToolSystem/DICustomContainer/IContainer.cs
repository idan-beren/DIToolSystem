namespace DIToolSystem.DICustomContainer;

public interface IContainer
{
    public IContainer Register<TInterface, TImplementation>()
        where TInterface : class where TImplementation : class, TInterface;
    
    public IContainer Register<TInterface>(Func<IContainer, TInterface> factory) 
        where TInterface : class;
    
    public IContainer Register<TImplementation>()
        where TImplementation : class;
}