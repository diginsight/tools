namespace DiginsightCopilotApi;

public static class LogStrings
{
    public static string? GetLogString(this string logString)
    {
        if (logString == null) { return null; }
        return logString.Replace("\r", "\\r").Replace("\n", "\\n");
    }
}
