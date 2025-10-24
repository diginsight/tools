namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// Atom link construct with relationship types
/// </summary>
public class AtomLink
{
    /// <summary>
    /// IRI reference (required)
    /// </summary>
    public string Href { get; set; }

    /// <summary>
    /// Link relationship type (alternate, enclosure, self, related, via, hub)
    /// </summary>
    public string Relation { get; set; } = "alternate";

    /// <summary>
    /// MIME media type
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Language of linked resource
    /// </summary>
    public string HrefLang { get; set; }

    /// <summary>
    /// Human-readable title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Size in bytes (for enclosures)
    /// </summary>
    public long? Length { get; set; }
}
