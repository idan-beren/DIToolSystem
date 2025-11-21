namespace DIToolSystem.ToolSystem.Tools;

public interface ITool
{
    public string Name { get; }
    public void Execute();
}