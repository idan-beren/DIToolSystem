namespace DIToolSystem.DICustomContainer;

public class RegistrationIndexer(Type serviceType)
{
    public Type ServiceType { get; } = serviceType;

    public string? Name { get; set; }
}