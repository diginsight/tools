using System.Xml.Linq;
using Diginsight.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Diginsight.Tools.FeedMonitor;

public class AtomFeedParser : FeedParserBase
{
    private readonly ILogger<AtomFeedParser> logger;

    public AtomFeedParser(ILogger<AtomFeedParser> logger = null!)
    {
        this.logger = logger;
    }

    public override FeedType SupportedFeedType => FeedType.Atom;

    public override bool CanParse(XDocument document)
    {
        return document.Root?.Name.LocalName.Equals("feed", StringComparison.OrdinalIgnoreCase) == true;
    }

    public override FeedChannelBase ParseFeed(string xmlContent)
    {
        using var activity = logger != null ? Observability.ActivitySource.StartMethodActivity(logger, () => new { }) : null;

        FeedChannelBase result = ParseAtom(xmlContent);
        
        activity?.SetOutput(result);
        return result;
    }

    public static AtomFeedChannel ParseAtom(string xmlContent)
    {
        var logger = Observability.LoggerFactory?.CreateLogger<AtomFeedParser>() ?? NullLogger<AtomFeedParser>.Instance;
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { xmlContent });

        var doc = XDocument.Parse(xmlContent);
        var ns = doc.Root.GetDefaultNamespace();
        var feed = doc.Root;

        if (feed.Name.LocalName != "feed")
            throw new InvalidOperationException("Invalid Atom feed: <feed> element not found");

        var atomFeed = new AtomFeedChannel
        {
            Id = feed.Element(ns + "id")?.Value,
            Title = feed.Element(ns + "title")?.Value,
            Description = feed.Element(ns + "subtitle")?.Value,
            Subtitle = feed.Element(ns + "subtitle")?.Value,
            Copyright = feed.Element(ns + "rights")?.Value,
            Icon = feed.Element(ns + "icon")?.Value,
            Logo = feed.Element(ns + "logo")?.Value,

            // Dates
            LastUpdated = ParseRFC3339Date(feed.Element(ns + "updated")?.Value)
        };

        // Parse generator
        var genElement = feed.Element(ns + "generator");
        if (genElement != null)
        {
            var generator = new AtomGenerator
            {
                Text = genElement.Value,
                Uri = genElement.Attribute("uri")?.Value,
                Version = genElement.Attribute("version")?.Value
            };
            atomFeed.Generator = generator;
            // Also set the base class Generator property for consistency
            ((FeedChannelBase)atomFeed).Generator = generator.Text;
        }

        // Parse authors
        foreach (var author in feed.Elements(ns + "author"))
        {
            atomFeed.Authors.Add(ParseAtomPerson(author, ns));
        }

        // Parse contributors
        foreach (var contributor in feed.Elements(ns + "contributor"))
        {
            atomFeed.Contributors.Add(ParseAtomPerson(contributor, ns));
        }

        // Parse links
        foreach (var link in feed.Elements(ns + "link"))
        {
            atomFeed.Links.Add(ParseAtomLink(link));
        }

        // Set main link (alternate)
        atomFeed.Link = atomFeed.Links
            .FirstOrDefault(l => l.Relation == "alternate" && l.Type == "text/html")?
            .Href;

        // Parse categories
        foreach (var cat in feed.Elements(ns + "category"))
        {
            atomFeed.Categories.Add(ParseAtomCategory(cat));
        }

        // Parse entries
        foreach (var entry in feed.Elements(ns + "entry"))
        {
            atomFeed.Items.Add(ParseAtomEntry(entry, ns));
        }

        activity?.SetOutput(atomFeed);
        return atomFeed;
    }

    private static AtomFeedItem ParseAtomEntry(XElement entry, XNamespace ns)
    {
        var atomItem = new AtomFeedItem
        {
            // Use the Atom id value directly as the string Id (typically a URL or URN)
            Id = entry.Element(ns + "id")?.Value,
            
            Title = entry.Element(ns + "title")?.Value,
            Rights = entry.Element(ns + "rights")?.Value,

            // Dates
            LastUpdated = ParseRFC3339Date(entry.Element(ns + "updated")?.Value),
            Published = ParseRFC3339Date(entry.Element(ns + "published")?.Value)
        };

        // Use published date as publication date
        atomItem.PublicationDate = atomItem.Published ?? atomItem.LastUpdated;

        // Parse summary
        var summaryElement = entry.Element(ns + "summary");
        if (summaryElement != null)
        {
            atomItem.Summary = new AtomText
            {
                Type = summaryElement.Attribute("type")?.Value ?? "text",
                Text = summaryElement.Value
            };
            atomItem.Description = atomItem.Summary.Text;
        }

        // Parse content
        var contentElement = entry.Element(ns + "content");
        if (contentElement != null)
        {
            atomItem.Content = new AtomContent
            {
                Type = contentElement.Attribute("type")?.Value ?? "text",
                Src = contentElement.Attribute("src")?.Value,
                Text = contentElement.Value
            };
        }

        // Parse authors
        foreach (var author in entry.Elements(ns + "author"))
        {
            atomItem.Authors.Add(ParseAtomPerson(author, ns));
        }

        // Set author string
        atomItem.Author = string.Join(", ", atomItem.Authors.Select(a => a.Name));

        // Parse contributors
        foreach (var contributor in entry.Elements(ns + "contributor"))
        {
            atomItem.Contributors.Add(ParseAtomPerson(contributor, ns));
        }

        // Parse links
        foreach (var link in entry.Elements(ns + "link"))
        {
            var atomLink = ParseAtomLink(link);
            atomItem.Links.Add(atomLink);

            // Handle enclosures
            if (atomLink.Relation == "enclosure")
            {
                atomItem.Enclosures.Add(new MediaEnclosure
                {
                    Url = atomLink.Href,
                    Type = atomLink.Type,
                    Length = atomLink.Length
                });
            }
        }

        // Set main link (alternate)
        atomItem.Link = atomItem.Links
            .FirstOrDefault(l => l.Relation == "alternate")?
            .Href;

        // Parse categories
        foreach (var cat in entry.Elements(ns + "category"))
        {
            atomItem.Categories.Add(ParseAtomCategory(cat));
        }

        return atomItem;
    }

    private static AtomPerson ParseAtomPerson(XElement person, XNamespace ns)
    {
        return new AtomPerson
        {
            Name = person.Element(ns + "name")?.Value,
            Uri = person.Element(ns + "uri")?.Value,
            Email = person.Element(ns + "email")?.Value
        };
    }

    private static AtomLink ParseAtomLink(XElement link)
    {
        return new AtomLink
        {
            Href = link.Attribute("href")?.Value,
            Relation = link.Attribute("rel")?.Value ?? "alternate",
            Type = link.Attribute("type")?.Value,
            HrefLang = link.Attribute("hreflang")?.Value,
            Title = link.Attribute("title")?.Value,
            Length = long.TryParse(link.Attribute("length")?.Value, out long len) ? len : (long?)null
        };
    }

    private static AtomCategory ParseAtomCategory(XElement category)
    {
        return new AtomCategory
        {
            Term = category.Attribute("term")?.Value,
            Scheme = category.Attribute("scheme")?.Value,
            Label = category.Attribute("label")?.Value
        };
    }

    private static DateTime? ParseRFC3339Date(string dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return null;

        try
        {
            return DateTime.Parse(dateString,
                null,
                System.Globalization.DateTimeStyles.RoundtripKind);
        }
        catch
        {
            return null;
        }
    }
}
