namespace DIToolSystem.DICustomContainer;

public interface IContainer
{
    public IContainer Register<TInterface, TImplementation>()
        where TInterface : class
        where TImplementation : class, TInterface;

    public IContainer Register<TInterface>(Func<IContainer, TInterface> factory)
        where TInterface : class;

    public IContainer RegisterType<TImplementation>()
        where TImplementation : class;

    public IContainer SingleInstance();

    public T Resolve<T>() where T : class;

    public Task<T> ResolveAsync<T>() where T : class;
}