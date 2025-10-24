using Diginsight.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// Factory service for creating appropriate feed parsers
/// </summary>
public class FeedParserFactory : IFeedParserFactory
{
    private readonly ILogger<FeedParserFactory> logger;
    private readonly RSSFeedParser rssFeedParser;
    private readonly AtomFeedParser atomFeedParser;

    /// <summary>
    /// Initializes a new instance of the FeedParserFactory class
    /// </summary>
    /// <param name="logger">Logger instance for observability</param>
    /// <param name="rssFeedParser">RSS feed parser instance</param>
    /// <param name="atomFeedParser">Atom feed parser instance</param>
    public FeedParserFactory(
        ILogger<FeedParserFactory> logger,
        RSSFeedParser rssFeedParser,
        AtomFeedParser atomFeedParser)
    {
        this.logger = logger;
        this.rssFeedParser = rssFeedParser;
        this.atomFeedParser = atomFeedParser;
    }

    /// <summary>
    /// Creates a parser and parses the feed in one step
    /// </summary>
    /// <param name="xmlContent">The XML content to parse</param>
    /// <returns>A parsed feed channel</returns>
    /// <exception cref="InvalidOperationException">Thrown when the feed format is not supported</exception>
    public FeedChannelBase ParseFeed(string xmlContent)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger);

        try
        {
            var parser = GetParser(xmlContent);
            FeedChannelBase result = parser.ParseFeed(xmlContent);

            activity?.SetOutput(result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while parsing feed");
            throw;
        }
    }

    /// <summary>
    /// Gets a parser for the specified XML content
    /// </summary>
    /// <param name="xmlContent">The XML content to analyze</param>
    /// <returns>A parser that can handle the feed format</returns>
    /// <exception cref="InvalidOperationException">Thrown when the feed format is not supported</exception>
    public FeedParserBase GetParser(string xmlContent)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger);

        try
        {
            var doc = XDocument.Parse(xmlContent);
            FeedParserBase result = GetParser(doc);

            activity?.SetOutput(result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while getting parser for XML content");
            throw;
        }
    }

    /// <summary>
    /// Gets a parser for the specified XML document
    /// </summary>
    /// <param name="document">The XML document to analyze</param>
    /// <returns>A parser that can handle the feed format</returns>
    /// <exception cref="InvalidOperationException">Thrown when the feed format is not supported</exception>
    public FeedParserBase GetParser(XDocument document)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger);

        try
        {
            FeedParserBase result;

            if (rssFeedParser.CanParse(document))
            {
                result = rssFeedParser;
            }
            else if (atomFeedParser.CanParse(document))
            {
                result = atomFeedParser;
            }
            else
            {
                throw new InvalidOperationException("Unknown feed format. Expected RSS or Atom.");
            }

            activity?.SetOutput(result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while getting parser for XML document");
            throw;
        }
    }

    /// <summary>
    /// Creates a parser based on the detected feed type
    /// </summary>
    /// <param name="feedType">The feed type</param>
    /// <returns>A parser for the specified feed type</returns>
    /// <exception cref="ArgumentException">Thrown when the feed type is not supported</exception>
    public FeedParserBase CreateParser(FeedType feedType)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { feedType });

        try
        {
            FeedParserBase result = feedType switch
            {
                FeedType.RSS20 => rssFeedParser,
                FeedType.Atom => atomFeedParser,
                _ => throw new ArgumentException($"Unsupported feed type: {feedType}", nameof(feedType))
            };

            activity?.SetOutput(result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while creating parser for feed type {FeedType}", feedType);
            throw;
        }
    }

    /// <summary>
    /// Determines the feed type from XML content
    /// </summary>
    /// <param name="xmlContent">The XML content to analyze</param>
    /// <returns>The detected feed type</returns>
    /// <exception cref="InvalidOperationException">Thrown when the feed format is not recognized</exception>
    public FeedType DetectFeedType(string xmlContent)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger);

        try
        {
            var doc = XDocument.Parse(xmlContent);
            FeedType result = DetectFeedType(doc);

            activity?.SetOutput(result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while detecting feed type from XML content");
            throw;
        }
    }

    /// <summary>
    /// Determines the feed type from XML document
    /// </summary>
    /// <param name="document">The XML document to analyze</param>
    /// <returns>The detected feed type</returns>
    /// <exception cref="InvalidOperationException">Thrown when the feed format is not recognized</exception>
    public FeedType DetectFeedType(XDocument document)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger);

        try
        {
            FeedType result;

            if (document.Root?.Name.LocalName.Equals("rss", StringComparison.OrdinalIgnoreCase) == true)
            {
                result = FeedType.RSS20;
            }
            else if (document.Root?.Name.LocalName.Equals("feed", StringComparison.OrdinalIgnoreCase) == true)
            {
                result = FeedType.Atom;
            }
            else
            {
                throw new InvalidOperationException("Unknown feed format. Expected RSS or Atom.");
            }

            activity?.SetOutput(result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while detecting feed type from XML document");
            throw;
        }
    }
}