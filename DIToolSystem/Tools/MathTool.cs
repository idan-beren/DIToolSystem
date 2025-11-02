using DIToolSystem.Tools.Utils;

namespace DIToolSystem.Tools;

public class MathTool(ILogger logger) : ITool
{
    public string Name => "MathTool";

    public void Execute()
    {
        logger.Log("Execute");
    }
}