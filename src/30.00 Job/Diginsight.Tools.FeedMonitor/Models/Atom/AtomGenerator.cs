namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// Atom generator information
/// </summary>
public class AtomGenerator
{
    /// <summary>
    /// Generator name/text
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Generator URI
    /// </summary>
    public string Uri { get; set; }

    /// <summary>
    /// Generator version
    /// </summary>
    public string Version { get; set; }

    public override string ToString() =>
        string.IsNullOrEmpty(Version) ? Text : $"{Text} {Version}";
}
