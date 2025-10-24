using System.Xml.Linq;

namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// Abstract base class for feed parsers
/// </summary>
public abstract class FeedParserBase
{
    /// <summary>
    /// Parses feed content and returns a feed channel
    /// </summary>
    /// <param name="xmlContent">The XML content of the feed</param>
    /// <returns>A FeedChannelBase instance containing the parsed feed data</returns>
    public abstract FeedChannelBase ParseFeed(string xmlContent);

    /// <summary>
    /// Gets the feed type that this parser handles
    /// </summary>
    public abstract FeedType SupportedFeedType { get; }

    /// <summary>
    /// Determines if this parser can handle the given XML document
    /// </summary>
    /// <param name="document">The XML document to check</param>
    /// <returns>True if this parser can handle the document, false otherwise</returns>
    public abstract bool CanParse(XDocument document);
}