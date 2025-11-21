using DIToolSystem.Tools.Utils;

namespace DIToolSystem.Tools;

public class HelloWorldTool(ILogger logger) : ITool
{
    public string Name => "HelloWorld";
    
    public void Execute()
    {
        logger.Log("Hello World!");
    }
}