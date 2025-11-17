namespace DIToolSystem.DICustomContainer;

public interface IContainer
{
    public IContainer Register<TImplementation>()
        where TImplementation : class;
    
    public IContainer Register<TImplementation, TInterface>()
        where TInterface : class
        where TImplementation : class, TInterface;

    public IContainer Register<TService>(Func<IContainer, TService> factory)
        where TService : class;

    public IContainer SingleInstance();

    public T Resolve<T>() where T : class;

    public Task<T> ResolveAsync<T>() where T : class;
}