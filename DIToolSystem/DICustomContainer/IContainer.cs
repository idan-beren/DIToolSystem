namespace DIToolSystem.DICustomContainer;

public interface IContainer
{
    public IContainer RegisterType<TImplementation>()
        where TImplementation : class;
    
    public IContainer Register<TImplementation, TInterface>()
        where TInterface : class
        where TImplementation : class, TInterface;

    public IContainer Register<TService>(Func<IContainer, TService> factory)
        where TService : class;

    public IContainer SingleInstance();
    
    public IContainer Named ( string name );

    public TService Resolve<TService>() where TService : class;
    
    public TService ResolveNamed<TService>(string name) where TService : class;
    
    public IEnumerable<TService> ResolveAll<TService>() where TService : class;

    public Task<TService> ResolveAsync<TService>() where TService : class;
}