using Azure.Monitor.Query.Models;
using Diginsight.Options;
using System.Text.Json.Serialization;

namespace Diginsight.Tools.FeedMonitor;

public class FeedMonitorConfiguration : IDynamicallyConfigurable, IVolatilelyConfigurable
{

    /// <summary>
    /// Azure AD Tenant ID for authentication
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Azure AD Client ID for authentication
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Azure AD Client Secret for authentication
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Determines whether to load existing feeds data
    /// </summary>
    public bool LoadExistingFeeds { get; set; } = true;

    /// <summary>
    /// Minimum interval in minutes for loading existing feeds
    /// </summary>
    public string LoadExistingFeedsInterval { get; set; } = "1M";

    /// <summary>
    /// Scheduling interval in minutes for runs
    /// </summary>
    public int RunsSchedulingInMinutes { get; set; } = 5;

    /// <summary>
    /// Time To Live for MetricValue entries, within CosmosDB, in seconds (default: 86400 seconds = 1 day)
    /// </summary>
    public int TimeToLive { get; set; } = 86400;

    /// <summary>
    /// Collection of Azure resources to monitor with their respective metrics
    /// </summary>
    public FeedItem[] Feeds { get; set; } = [];
}

/// <summary>
/// Configuration for a specific Azure resource to monitor
/// </summary>
public class FeedItem
{
    /// <summary>
    /// The Azure resource ID to monitor
    /// </summary>
    public string Uri { get; set; } = string.Empty;

}
