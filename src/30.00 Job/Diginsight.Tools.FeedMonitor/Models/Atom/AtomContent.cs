namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// Atom content construct
/// </summary>
public class AtomContent
{
    /// <summary>
    /// Content type (text, html, xhtml, or MIME type)
    /// </summary>
    public string Type { get; set; } = "text";

    /// <summary>
    /// Content text/markup
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// External content IRI (for out-of-line content)
    /// </summary>
    public string Src { get; set; }

    /// <summary>
    /// Whether content is inline or external
    /// </summary>
    public bool IsInline => string.IsNullOrEmpty(Src);
}
