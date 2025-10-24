namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// Atom syndication feed implementation (RFC 4287)
/// </summary>
public class AtomFeedChannel : FeedChannelBase
{
    /// <summary>
    /// Feed subtitle/tagline
    /// </summary>
    public string Subtitle { get; set; }

    /// <summary>
    /// Feed authors
    /// </summary>
    public List<AtomPerson> Authors { get; set; } = new List<AtomPerson>();

    /// <summary>
    /// Feed contributors
    /// </summary>
    public List<AtomPerson> Contributors { get; set; } = new List<AtomPerson>();

    /// <summary>
    /// Feed links (alternate, self, related, hub)
    /// </summary>
    public List<AtomLink> Links { get; set; } = new List<AtomLink>();

    /// <summary>
    /// Feed categories with schemes
    /// </summary>
    public new List<AtomCategory> Categories { get; set; } = new List<AtomCategory>();

    /// <summary>
    /// Feed icon URL (small, 1:1 aspect ratio)
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// Feed logo URL (larger, 2:1 aspect ratio)
    /// </summary>
    public string Logo { get; set; }

    /// <summary>
    /// Generator information
    /// </summary>
    public new AtomGenerator Generator { get; set; }

    /// <summary>
    /// WebSub hub URL (from link rel="hub")
    /// </summary>
    public string WebSubHub
    {
        get => Links?.Find(l => l.Relation == "hub")?.Href;
    }

    /// <summary>
    /// Self-reference URL (from link rel="self")
    /// </summary>
    public string SelfLink
    {
        get => Links?.Find(l => l.Relation == "self")?.Href;
    }

    public override FeedType FeedType => FeedType.Atom;
}
