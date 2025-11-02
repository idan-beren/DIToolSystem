namespace DIToolSystem.DICustomContainer;

public class Container
{
    private readonly Dictionary<Type, Type> _registrations = new();
    
    public void Register<TInterface, TImplementation>()
    {
        _registrations[typeof(TInterface)] = typeof(TImplementation);
    }

    public TInterface Resolve<TInterface>()
    {
        return (TInterface)Resolve(typeof(TInterface));
    }
    
    private object Resolve(Type type)
    {
        if (!_registrations.TryGetValue(type, out var implementationType))
            throw new Exception($"Type {type.Name} not registered");

        var constructor = implementationType.GetConstructors().First();
        var parameters = constructor.GetParameters()
            .Select(p => Resolve(p.ParameterType))
            .ToArray();

        return Activator.CreateInstance(implementationType, parameters)!;
    }
    
}