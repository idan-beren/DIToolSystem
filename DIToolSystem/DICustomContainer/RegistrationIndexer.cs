namespace DIToolSystem.DICustomContainer;

public class RegistrationIndexer(Type serviceType) : IComparable, ICloneable
{
    public Type ServiceType { get; } = serviceType;

    public string? Name { get; set; }
    
    public int CompareTo(object? obj)
    {
        if (obj is not RegistrationIndexer o) 
            throw new ArgumentException("Object is not a RegistrationIndexer");
        
        var result = string.Compare(ServiceType.FullName, o.ServiceType.FullName, StringComparison.Ordinal);
        return result != 0 ? result : string.Compare(Name, o.Name, StringComparison.Ordinal);
    }

    public object Clone()
    {
        return new RegistrationIndexer(ServiceType) { Name = Name };
    }
}