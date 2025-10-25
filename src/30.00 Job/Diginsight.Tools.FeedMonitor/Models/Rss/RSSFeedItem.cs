namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// RSS 2.0 item implementation with iTunes extensions
/// </summary>
public class RSSFeedItem : FeedItemBase
{
    /// <summary>
    /// Whether GUID is a permalink
    /// </summary>
    public bool GuidIsPermaLink { get; set; } = true;

    /// <summary>
    /// URL to comments page
    /// </summary>
    public string Comments { get; set; }

    /// <summary>
    /// Source feed information (for aggregated content)
    /// </summary>
    public RSSSource Source { get; set; }

    // Extended RSS Elements (Dublin Core, Content, etc.)
    /// <summary>
    /// Dublin Core creator/author (dc:creator)
    /// </summary>
    public string DcCreator { get; set; }

    /// <summary>
    /// Full HTML content (content:encoded)
    /// </summary>
    public string ContentEncoded { get; set; }

    /// <summary>
    /// Comment RSS feed URL (wfw:commentRss)
    /// </summary>
    public string CommentRssUrl { get; set; }

    /// <summary>
    /// Number of comments (slash:comments)
    /// </summary>
    public int? CommentCount { get; set; }

    // Item-level image (non-standard but widely used)
    /// <summary>
    /// Item-level image URL (non-standard extension)
    /// </summary>
    public string ImageUrl { get; set; }

    /// <summary>
    /// Item-level image MIME type (non-standard extension)
    /// </summary>
    public string ImageType { get; set; }

    // iTunes Episode Extensions
    /// <summary>
    /// iTunes episode author
    /// </summary>
    public string ItunesAuthor { get; set; }

    /// <summary>
    /// iTunes episode subtitle
    /// </summary>
    public string ItunesSubtitle { get; set; }

    /// <summary>
    /// iTunes episode summary
    /// </summary>
    public string ItunesSummary { get; set; }

    /// <summary>
    /// iTunes episode duration
    /// </summary>
    public TimeSpan? ItunesDuration { get; set; }

    /// <summary>
    /// iTunes episode explicit rating
    /// </summary>
    public bool? ItunesExplicit { get; set; }

    /// <summary>
    /// iTunes episode artwork URL
    /// </summary>
    public string ItunesImageUrl { get; set; }

    /// <summary>
    /// iTunes episode number
    /// </summary>
    public int? ItunesEpisode { get; set; }

    /// <summary>
    /// iTunes season number
    /// </summary>
    public int? ItunesSeason { get; set; }

    /// <summary>
    /// iTunes episode type (full, trailer, bonus)
    /// </summary>
    public string ItunesEpisodeType { get; set; }

    public override FeedType ItemType => FeedType.RSS20;
}
