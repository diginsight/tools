namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// RSS 2.0 channel/feed implementation with iTunes extensions
/// </summary>
public class RSSFeedChannel : FeedChannelBase
{
    /// <summary>
    /// Managing editor email address
    /// </summary>
    public string ManagingEditor { get; set; }

    /// <summary>
    /// Webmaster email address
    /// </summary>
    public string WebMaster { get; set; }

    /// <summary>
    /// Time-to-live in minutes (caching hint)
    /// </summary>
    public int? Ttl { get; set; }

    /// <summary>
    /// Hours when aggregators should skip updates (0-23)
    /// </summary>
    public List<int> SkipHours { get; set; } = new List<int>();

    /// <summary>
    /// Days when aggregators should skip updates
    /// </summary>
    public List<string> SkipDays { get; set; } = new List<string>();

    /// <summary>
    /// URL to RSS specification documentation
    /// </summary>
    public string Docs { get; set; }

    /// <summary>
    /// Cloud notification settings (RSSCloud)
    /// </summary>
    public CloudSettings Cloud { get; set; }

    /// <summary>
    /// WebSub hub URL for push notifications
    /// </summary>
    public string WebSubHub { get; set; }

    /// <summary>
    /// Self-reference URL (for WebSub)
    /// </summary>
    public string SelfLink { get; set; }

    // iTunes Podcast Extensions
    /// <summary>
    /// iTunes podcast author
    /// </summary>
    public string ItunesAuthor { get; set; }

    /// <summary>
    /// iTunes podcast subtitle
    /// </summary>
    public string ItunesSubtitle { get; set; }

    /// <summary>
    /// iTunes podcast summary
    /// </summary>
    public string ItunesSummary { get; set; }

    /// <summary>
    /// iTunes explicit content rating
    /// </summary>
    public bool? ItunesExplicit { get; set; }

    /// <summary>
    /// iTunes podcast type (episodic or serial)
    /// </summary>
    public string ItunesType { get; set; }

    /// <summary>
    /// iTunes owner information
    /// </summary>
    public ItunesOwner ItunesOwner { get; set; }

    /// <summary>
    /// iTunes artwork URL
    /// </summary>
    public string ItunesImageUrl { get; set; }

    /// <summary>
    /// iTunes categories
    /// </summary>
    public List<ItunesCategory> ItunesCategories { get; set; } = new List<ItunesCategory>();

    public override FeedType FeedType => FeedType.RSS20;
}
