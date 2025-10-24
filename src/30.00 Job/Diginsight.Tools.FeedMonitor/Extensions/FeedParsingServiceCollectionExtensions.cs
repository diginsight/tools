using Microsoft.Extensions.DependencyInjection;

namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// Extension methods for configuring feed parsing services
/// </summary>
public static class FeedParsingServiceCollectionExtensions
{
    /// <summary>
    /// Adds feed parsing services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddFeedParsing(this IServiceCollection services)
    {
        services.AddSingleton<RSSFeedParser>();
        services.AddSingleton<AtomFeedParser>();
        services.AddSingleton<IFeedParserFactory, FeedParserFactory>();
        return services;
    }
}