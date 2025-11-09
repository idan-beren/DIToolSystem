using DIToolSystem.DICustomContainer;
using DIToolSystem.Tools;
using DIToolSystem.Tools.Utils;

var container = new Container();
container.Register<ITool, MathTool>();
container.Register<ILogger, ConsoleLogger>(ServiceLifetime.Singleton);
var t1 = container.Resolve<ITool>();
t1.Execute();
