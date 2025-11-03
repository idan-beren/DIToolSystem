using DIToolSystem.DICustomContainer;
using DIToolSystem.Tools;
using DIToolSystem.Tools.Utils;

var container = new Container();
container.Register<ITool, MathTool>();
container.Register<ILogger, ConsoleLogger>(ServiceLifetime.Singleton);
var t1 = container.Resolve<ILogger>();
var t2 = container.Resolve<ILogger>();
Console.WriteLine(ReferenceEquals(t1, t2));
