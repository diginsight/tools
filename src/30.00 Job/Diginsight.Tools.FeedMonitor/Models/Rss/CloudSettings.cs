namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// RSS Cloud notification settings
/// </summary>
public class CloudSettings
{
    public string Domain { get; set; }
    public int Port { get; set; }
    public string Path { get; set; }
    public string RegisterProcedure { get; set; }
    public string Protocol { get; set; }
}
