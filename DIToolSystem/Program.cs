using DIToolSystem.DICustomContainer;
using DIToolSystem.Tools;
using DIToolSystem.Tools.Utils;

var container = new Container();
container.Register<ITool, MathTool>();
container.Register<ILogger, ConsoleLogger>(ServiceLifetime.Singleton);
var t1 = container.Resolve<ITool>();
t1.Execute();

Console.WriteLine("4. Lazy<T> Support:");
var lazyLogger = container.Resolve<Lazy<ILogger>>();
Console.WriteLine($"   Lazy<ILogger> created: {lazyLogger != null}");
Console.WriteLine($"   Lazy value accessed: {lazyLogger?.Value != null}");
Console.WriteLine();