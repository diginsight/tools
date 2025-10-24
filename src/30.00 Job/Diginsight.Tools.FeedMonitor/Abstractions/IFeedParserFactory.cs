using System.Xml.Linq;

namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// Interface for feed parser factory service
/// </summary>
public interface IFeedParserFactory
{
    /// <summary>
    /// Creates a parser and parses the feed in one step
    /// </summary>
    /// <param name="xmlContent">The XML content to parse</param>
    /// <returns>A parsed feed channel</returns>
    /// <exception cref="InvalidOperationException">Thrown when the feed format is not supported</exception>
    FeedChannelBase ParseFeed(string xmlContent);

    /// <summary>
    /// Gets a parser for the specified XML content
    /// </summary>
    /// <param name="xmlContent">The XML content to analyze</param>
    /// <returns>A parser that can handle the feed format</returns>
    /// <exception cref="InvalidOperationException">Thrown when the feed format is not supported</exception>
    FeedParserBase GetParser(string xmlContent);

    /// <summary>
    /// Gets a parser for the specified XML document
    /// </summary>
    /// <param name="document">The XML document to analyze</param>
    /// <returns>A parser that can handle the feed format</returns>
    /// <exception cref="InvalidOperationException">Thrown when the feed format is not supported</exception>
    FeedParserBase GetParser(XDocument document);

    /// <summary>
    /// Creates a parser based on the detected feed type
    /// </summary>
    /// <param name="feedType">The feed type</param>
    /// <returns>A parser for the specified feed type</returns>
    /// <exception cref="ArgumentException">Thrown when the feed type is not supported</exception>
    FeedParserBase CreateParser(FeedType feedType);

    /// <summary>
    /// Determines the feed type from XML content
    /// </summary>
    /// <param name="xmlContent">The XML content to analyze</param>
    /// <returns>The detected feed type</returns>
    /// <exception cref="InvalidOperationException">Thrown when the feed format is not recognized</exception>
    FeedType DetectFeedType(string xmlContent);

    /// <summary>
    /// Determines the feed type from XML document
    /// </summary>
    /// <param name="document">The XML document to analyze</param>
    /// <returns>The detected feed type</returns>
    /// <exception cref="InvalidOperationException">Thrown when the feed format is not recognized</exception>
    FeedType DetectFeedType(XDocument document);
}