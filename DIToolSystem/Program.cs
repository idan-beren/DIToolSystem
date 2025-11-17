using DIToolSystem.DICustomContainer;
using DIToolSystem.Tools;
using DIToolSystem.Tools.Utils;

namespace DIToolSystem;

public class ToolRunner(IContainer container)
{
    public void CreateAllTools()
    {
        container.Register<ConsoleLogger, ILogger>().SingleInstance();
        container.Register<HelloWorldTool, ITool>().Named("HelloWorldTool");
        container.Register<MathTool, ITool>().Named("MathTool");
    }

    public void RunAllTools()
    {
        var allTools = container.ResolveAll<ITool>();
        foreach (var tool in allTools)
            tool.Execute();
    }
}

public static class Program
{
    private static void Main(string[] args)
    {
        var container = new Container();
        var toolRunner = new ToolRunner(container);
        toolRunner.CreateAllTools();
        toolRunner.RunAllTools();
    }
}