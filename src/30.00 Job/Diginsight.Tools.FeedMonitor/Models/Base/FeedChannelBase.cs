namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// Enumeration of supported feed types
/// </summary>
public enum FeedType
{
    RSS20,
    Atom
}

/// <summary>
/// Abstract base class for feed-level metadata (channel/feed)
/// </summary>
public abstract class FeedChannelBase
{
    /// <summary>
    /// Unique identifier for the feed
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Human-readable feed title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Feed description or subtitle
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Website URL associated with the feed
    /// </summary>
    public string Link { get; set; }

    /// <summary>
    /// Language code (e.g., "en-US", "fr-FR")
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Copyright/rights information
    /// </summary>
    public string Copyright { get; set; }

    /// <summary>
    /// Last update/modification timestamp
    /// </summary>
    public DateTime? LastUpdated { get; set; }

    /// <summary>
    /// Publication date
    /// </summary>
    public DateTime? PublicationDate { get; set; }

    /// <summary>
    /// Feed categories/tags
    /// </summary>
    public List<string> Categories { get; set; } = new List<string>();

    /// <summary>
    /// Feed image/logo URL
    /// </summary>
    public string ImageUrl { get; set; }

    /// <summary>
    /// Software that generated the feed
    /// </summary>
    public string Generator { get; set; }

    /// <summary>
    /// Collection of feed items/entries
    /// </summary>
    public List<FeedItemBase> Items { get; set; } = new List<FeedItemBase>();

    /// <summary>
    /// Feed format type
    /// </summary>
    public abstract FeedType FeedType { get; }
}
