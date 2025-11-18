using DIToolSystem.DICustomContainer;
using DIToolSystem.Tools;
using DIToolSystem.Tools.Utils;

namespace DIToolSystem;

public class ToolRunner(IContainer container)
{
    public void CreateAllTools()
    {
        container.RegisterAssembly<ITool>(typeof(ITool).Assembly);
        container.RegisterAssembly<ILogger>(typeof(ILogger).Assembly).SingleInstance();
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