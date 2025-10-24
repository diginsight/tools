using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Diginsight.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Diginsight.Tools.FeedMonitor;

public class RSSFeedParser : FeedParserBase
{
    private readonly ILogger<RSSFeedParser> logger;

    public RSSFeedParser(ILogger<RSSFeedParser> logger = null!)
    {
        this.logger = logger;
    }

    public override FeedType SupportedFeedType => FeedType.RSS20;

    public override bool CanParse(XDocument document)
    {
        return document.Root?.Name.LocalName.Equals("rss", StringComparison.OrdinalIgnoreCase) == true;
    }

    public override FeedChannelBase ParseFeed(string xmlContent)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { });

        FeedChannelBase result = ParseRSS(xmlContent);
        
        activity?.SetOutput(result);
        return result;
    }

    public static RSSFeedChannel ParseRSS(string xmlContent)
    {
        var logger = Observability.LoggerFactory?.CreateLogger<RSSFeedParser>() ?? NullLogger<RSSFeedParser>.Instance;
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { });

        var doc = XDocument.Parse(xmlContent);
        var channel = doc.Descendants("channel").FirstOrDefault();

        if (channel == null)
            throw new InvalidOperationException("Invalid RSS feed: <channel> element not found");

        var itunesNs = XNamespace.Get("http://www.itunes.com/dtds/podcast-1.0.dtd");
        var atomNs = XNamespace.Get("http://www.w3.org/2005/Atom");

        var rssFeed = new RSSFeedChannel
        {
            Id = channel.Element("link")?.Value!,
            Title = channel.Element("title")?.Value!,
            Description = channel.Element("description")?.Value!,
            Link = channel.Element("link")?.Value!,
            Language = channel.Element("language")?.Value!,
            Copyright = channel.Element("copyright")?.Value!,
            ManagingEditor = channel.Element("managingEditor")?.Value!,
            WebMaster = channel.Element("webMaster")?.Value!,
            Generator = channel.Element("generator")?.Value!,
            ImageUrl = channel.Element("image")?.Element("url")?.Value!,

            // Parse dates
            PublicationDate = ParseRFC822Date(channel.Element("pubDate")?.Value!),
            LastUpdated = ParseRFC822Date(channel.Element("lastBuildDate")?.Value!),

            // Parse TTL
            Ttl = int.TryParse(channel.Element("ttl")?.Value, out int ttl) ? ttl : (int?)null,

            // WebSub support
            WebSubHub = channel.Elements(atomNs + "link")
                .FirstOrDefault(l => (string)l.Attribute("rel") == "hub")?
                .Attribute("href")?.Value,
            SelfLink = channel.Elements(atomNs + "link")
                .FirstOrDefault(l => (string)l.Attribute("rel") == "self")?
                .Attribute("href")?.Value,

            // iTunes extensions
            ItunesAuthor = channel.Element(itunesNs + "author")?.Value!,
            ItunesSubtitle = channel.Element(itunesNs + "subtitle")?.Value!,
            ItunesSummary = channel.Element(itunesNs + "summary")?.Value!,
            ItunesImageUrl = channel.Element(itunesNs + "image")?.Attribute("href")?.Value!,
            ItunesExplicit = ParseItunesExplicit(channel.Element(itunesNs + "explicit")?.Value),
            ItunesType = channel.Element(itunesNs + "type")?.Value!
        };

        // Parse categories
        rssFeed.Categories.AddRange(
            channel.Elements("category").Select(c => c.Value)
        );

        // Parse items
        foreach (var item in channel.Elements("item"))
        {
            rssFeed.Items.Add(ParseRSSItem(item, itunesNs));
        }

        activity?.SetOutput(rssFeed);
        return rssFeed;
    }

    private static RSSFeedItem ParseRSSItem(XElement item, XNamespace itunesNs)
    {
        var logger = Observability.LoggerFactory?.CreateLogger<RSSFeedParser>() ?? NullLogger<RSSFeedParser>.Instance;
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { }, logLevel: LogLevel.Trace);

        var guidValue = item.Element("guid")?.Value;
        
        var rssItem = new RSSFeedItem
        {
            Guid = guidValue,

            Title = item.Element("title")?.Value!,
            Description = item.Element("description")?.Value!,
            Link = item.Element("link")?.Value!,
            Author = item.Element("author")?.Value!,
            Comments = item.Element("comments")?.Value!,

            // GUID metadata
            GuidIsPermaLink = item.Element("guid")?.Attribute("isPermaLink")?.Value != "false",

            // Dates
            PublicationDate = ParseRFC822Date(item.Element("pubDate")?.Value),

            // iTunes extensions
            ItunesAuthor = item.Element(itunesNs + "author")?.Value!,
            ItunesSubtitle = item.Element(itunesNs + "subtitle")?.Value!,
            ItunesSummary = item.Element(itunesNs + "summary")?.Value!,
            ItunesImageUrl = item.Element(itunesNs + "image")?.Attribute("href")?.Value!,
            ItunesExplicit = ParseItunesExplicit(item.Element(itunesNs + "explicit")?.Value),
            ItunesDuration = ParseItunesDuration(item.Element(itunesNs + "duration")?.Value),
            ItunesEpisode = int.TryParse(item.Element(itunesNs + "episode")?.Value, out int ep) ? ep : (int?)null,
            ItunesSeason = int.TryParse(item.Element(itunesNs + "season")?.Value, out int season) ? season : (int?)null,
            ItunesEpisodeType = item.Element(itunesNs + "episodeType")?.Value!
        };

        // Parse categories
        rssItem.Categories.AddRange(
            item.Elements("category").Select(c => c.Value)
        );

        // Parse enclosures
        foreach (var enc in item.Elements("enclosure"))
        {
            rssItem.Enclosures.Add(new MediaEnclosure
            {
                Url = enc.Attribute("url")?.Value!,
                Type = enc.Attribute("type")?.Value!,
                Length = long.TryParse(enc.Attribute("length")?.Value, out long len) ? len : (long?)null,
                Duration = rssItem.ItunesDuration
            });
        }

        return rssItem;
    }

    private static DateTime? ParseRFC822Date(string dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return null;

        try
        {
            return DateTime.Parse(dateString,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AdjustToUniversal);
        }
        catch
        {
            return null;
        }
    }

    private static bool? ParseItunesExplicit(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return value.ToLower() == "true" || value.ToLower() == "yes";
    }

    private static TimeSpan? ParseItunesDuration(string duration)
    {
        if (string.IsNullOrWhiteSpace(duration))
            return null;

        // Format: HH:MM:SS or MM:SS or seconds
        if (TimeSpan.TryParse(duration, out TimeSpan ts))
            return ts;

        if (int.TryParse(duration, out int seconds))
            return TimeSpan.FromSeconds(seconds);

        return null;
    }
}
