namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// Atom text construct (for summary, title, etc.)
/// </summary>
public class AtomText
{
    /// <summary>
    /// Text type (text, html, xhtml)
    /// </summary>
    public string Type { get; set; } = "text";

    /// <summary>
    /// Text content
    /// </summary>
    public string Text { get; set; }

    public override string ToString() => Text;
}
