namespace DIToolSystem.Tools.Runner;

public class ToolRunner : IRunner
{
    private Dictionary<string, ITool> Tools { get; } = new();

    public ToolRunner(IEnumerable<ITool> tools)
    {
        var enumerable = tools.ToList();
        for (var i = 0; i < enumerable.Count; i++)
        {
            Tools[(i + 1).ToString()] = enumerable[i];
        }
    }

    public void Show()
    {
        Console.WriteLine("Available tools:");
        foreach (var tool in Tools)
            Console.WriteLine($"{tool.Key}: {tool.Value.Name}");
    }

    public void Run()
    {
        Console.WriteLine("Choose number:");
        var tool = Console.ReadLine();
        if (tool != null && Tools.TryGetValue(tool, out var value))
            value.Execute();
        Console.WriteLine();
    }

    public bool IsToolsEmpty()
    {
        return Tools.Count == 0;
    }
}