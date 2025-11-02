namespace DIToolSystem.DICustomContainer;

public interface IContainer
{
    public void Register<TInterface, TImplementation>();
    public TInterface Resolve<TInterface>();
}