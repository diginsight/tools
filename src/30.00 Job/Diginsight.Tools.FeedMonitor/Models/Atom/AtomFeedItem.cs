namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// Atom entry implementation (RFC 4287)
/// </summary>
public class AtomFeedItem : FeedItemBase
{
    /// <summary>
    /// Entry authors
    /// </summary>
    public List<AtomPerson> Authors { get; set; } = new List<AtomPerson>();

    /// <summary>
    /// Entry contributors
    /// </summary>
    public List<AtomPerson> Contributors { get; set; } = new List<AtomPerson>();

    /// <summary>
    /// Entry links (alternate, enclosure, related)
    /// </summary>
    public List<AtomLink> Links { get; set; } = new List<AtomLink>();

    /// <summary>
    /// Entry categories with schemes
    /// </summary>
    public new List<AtomCategory> Categories { get; set; } = new List<AtomCategory>();

    /// <summary>
    /// Entry content
    /// </summary>
    public AtomContent Content { get; set; }

    /// <summary>
    /// Entry summary/excerpt
    /// </summary>
    public AtomText Summary { get; set; }

    /// <summary>
    /// Original publication timestamp
    /// </summary>
    public DateTime? Published { get; set; }

    /// <summary>
    /// Rights/copyright information
    /// </summary>
    public string Rights { get; set; }

    /// <summary>
    /// Source feed metadata (for aggregated entries)
    /// </summary>
    public AtomSource Source { get; set; }

    public override FeedType ItemType => FeedType.Atom;
}
