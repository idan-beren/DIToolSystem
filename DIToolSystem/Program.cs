using DIToolSystem.DICustomContainer;
using DIToolSystem.Tools;
using DIToolSystem.Tools.Utils;

var container = new Container();
container.Register<MathTool, ITool>().Named("Math1");
container.Register<MathTool, ITool>().Named("Math2");
container.Register<MathTool, ITool>().Named("Math3");
container.Register<ConsoleLogger, ILogger>().SingleInstance();

var t1 = container.ResolveNamed<ITool>("Math1");
var all = container.ResolveAll<ITool>();

foreach (var tool in all)
    tool.Execute();
t1.Execute();