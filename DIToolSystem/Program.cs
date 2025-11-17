using DIToolSystem.DICustomContainer;
using DIToolSystem.Tools;
using DIToolSystem.Tools.Utils;

var container = new Container();
container.Register<MathTool, ITool>().Named("Math");
container.Register<ConsoleLogger, ILogger>().SingleInstance();

var t1 = container.ResolveNamed<ITool>("Math");
t1.Execute();