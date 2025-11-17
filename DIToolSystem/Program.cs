using DIToolSystem.DICustomContainer;
using DIToolSystem.Tools;
using DIToolSystem.Tools.Utils;

var container = new Container();
container.Register<MathTool, ITool>();
container.Register<ConsoleLogger, ILogger>().SingleInstance();

var t1 = container.Resolve<ITool>();
t1.Execute();