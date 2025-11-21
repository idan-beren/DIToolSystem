using DIToolSystem.Tools.Utils;

namespace DIToolSystem.ToolSystem.Tools;

public class MathTool(ILogger logger) : ITool
{
    public string Name => "Math";

    public void Execute()
    {
        logger.Log("Enter an expression:");
        var expression = Console.ReadLine();
        var result = new System.Data.DataTable().Compute(expression, null);
        logger.Log($"Result: {result}");
    }
}