using DIToolSystem.DICustomContainer;
using DIToolSystem.Tools.Utils;
using DIToolSystem.ToolSystem.Runner;
using DIToolSystem.ToolSystem.Tools;

namespace DIToolSystem;

public static class Program
{
    private static readonly IContainer Container = new Container();
    
    private static void Main(string[] args)
    {
        RegisterTools();
        var runner = new ToolRunner(ResolveTools());
        while (!runner.IsToolsEmpty())
        {
            runner.Show();
            runner.Run();
        }
    }

    private static IEnumerable<ITool> ResolveTools()
    {
        return Container.ResolveAll<ITool>();
    }

    private static void RegisterTools()
    {
        Container.RegisterAssembly<ITool>(typeof(ITool).Assembly);
        Container.Register<ConsoleLogger, ILogger>().SingleInstance();
    }
}