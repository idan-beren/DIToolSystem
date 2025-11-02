using DIToolSystem.DICustomContainer;
using DIToolSystem.Tools;

var container = new Container();
container.Register<ITool, HelloWorldTool>();
var tool = container.Resolve<ITool>();
tool.Execute();
