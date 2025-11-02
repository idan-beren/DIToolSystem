namespace DIToolSystem.Tools;

public class HelloWorldTool : ITool
{
    public string Name => "HelloWorld";
    
    public void Execute()
    {
        Console.WriteLine("Hello-World!");
    }
}