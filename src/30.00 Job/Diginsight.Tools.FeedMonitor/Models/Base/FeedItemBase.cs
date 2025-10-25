using Diginsight.Tools.FeedMonitor;
using Newtonsoft.Json;

namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// Abstract base class for individual feed items/entries
/// </summary>
public abstract class FeedItemBase: EntityBase
{
    /// <summary>
    /// Feed Item unique Id
    /// </summary>
    public string Guid { get; set; }

    /// <summary>
    /// Item title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Item description or summary
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Permanent URL for the item
    /// </summary>
    public string Link { get; set; }

    /// <summary>
    /// Author information
    /// </summary>
    public string Author { get; set; }

    /// <summary>
    /// Publication date
    /// </summary>
    public DateTime? PublicationDate { get; set; }

    /// <summary>
    /// Last update/modification timestamp
    /// </summary>
    public DateTime? LastUpdated { get; set; }

    /// <summary>
    /// Item categories/tags
    /// </summary>
    public List<string> Categories { get; set; } = new List<string>();

    /// <summary>
    /// Media enclosures (audio, video, attachments)
    /// </summary>
    public List<MediaEnclosure> Enclosures { get; set; } = new List<MediaEnclosure>();

    /// <summary>
    /// Item format type
    /// </summary>
    public abstract FeedType ItemType { get; }

    /// <summary>
    /// Type discriminator for CosmosDB queries
    /// </summary>
    public string Type => this.GetType().Name;

    /// <summary>
    /// ID of the parent feed/channel
    /// </summary>
    public string FeedId { get; set; }

    /// <summary>
    /// Feed channel IDs where this item was discovered
    /// </summary>
    public List<string> FeedChannels { get; set; } = new List<string>();

    /// <summary>
    /// Partition key for CosmosDB (format: "feedSource-YYYY")
    /// </summary>
    [JsonProperty(PropertyName = "partitionKey", Required = Required.Always)]
    public string PartitionKey { get; set; }
}
