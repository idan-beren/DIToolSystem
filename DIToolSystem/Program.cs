using DIToolSystem.DICustomContainer;
using DIToolSystem.Tools;
using DIToolSystem.Tools.Utils;

var container = new Container();
container.Register<ITool, MathTool>();
container.Register<ILogger, ConsoleLogger>();
