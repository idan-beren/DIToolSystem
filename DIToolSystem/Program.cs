using DIToolSystem.DICustomContainer;
using DIToolSystem.Tools;
using DIToolSystem.Tools.Utils;

namespace DIToolSystem;

public class ToolRunner(IContainer container)
{
    public void CreateAllTools()
    {
        // Registering all tools to their interface via assembly registering
        container.RegisterAssembly<ITool>(typeof(ITool).Assembly);
        
        // Registering logger to its interface as singleton
        container.Register<ConsoleLogger, ILogger>().SingleInstance();
        
        // Registering logger named
        container.RegisterType<ConsoleLogger>().Named("Logger");
    }

    public void RunAllTools()
    {
        // Resolving logger named
        var logger = container.ResolveNamed<ConsoleLogger>("Logger");

        logger.Log("Starting...");
        
        // Resolving all tools according to the interface
        var tools = container.ResolveAll<ITool>();
        foreach (var tool in tools)
        {
            logger.Log($"Executing {tool.Name}:");
            tool.Execute();
        }
        
        logger.Log("All tools executed.");
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