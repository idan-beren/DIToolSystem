namespace DIToolSystem.Tools.Utils;

public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine(message);
}