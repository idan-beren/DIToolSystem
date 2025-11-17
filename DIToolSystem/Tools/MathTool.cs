using DIToolSystem.Tools.Utils;

namespace DIToolSystem.Tools;

public class MathTool(ILogger logger) : ITool
{
    public string Name => "MathTool";

    public void Execute()
    {
        var result = new System.Data.DataTable().Compute("1*1", null);
        logger.Log($"{result}");
    }
}