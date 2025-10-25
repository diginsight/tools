using Newtonsoft.Json;

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
public abstract class FeedChannelBase : EntityBase
{
    /// <summary>
    /// Feed URI (unique identifier for the source)
    /// </summary>
    public string Uri { get; set; }

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
    /// Collection of feed items/entries (not persisted - used only during parsing)
    /// </summary>
    [JsonIgnore]
    public List<FeedItemBase> Items { get; set; } = new List<FeedItemBase>();

    /// <summary>
    /// Feed format type
    /// </summary>
    public abstract FeedType FeedType { get; }

    /// <summary>
    /// Type discriminator for CosmosDB queries
    /// </summary>
    public string Type => this.GetType().Name;

    /// <summary>
    /// Category path for hierarchical feeds (e.g., "announcements", "updates/security")
    /// </summary>
    public string CategoryPath { get; set; }

    /// <summary>
    /// When this feed was first discovered/monitored
    /// </summary>
    public DateTimeOffset FirstDiscovered { get; set; }

    /// <summary>
    /// When this feed was last successfully fetched
    /// </summary>
    public DateTimeOffset LastSeen { get; set; }

    /// <summary>
    /// Partition key for CosmosDB (format: "feedDomain")
    /// </summary>
    [JsonProperty(PropertyName = "partitionKey", Required = Required.Always)]
    public string PartitionKey { get; set; }
}
