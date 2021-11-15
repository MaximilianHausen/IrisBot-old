using Microsoft.Extensions.Logging;

namespace IrisLoader;

public static class Logger
{
    public static void Log(LogLevel logLevel, int sourceId, string sourceName, string message)
    {
        Loader.Client.Logger.Log(logLevel, new EventId(sourceId, sourceName), message, null, (s, e) => s);
    }
}
