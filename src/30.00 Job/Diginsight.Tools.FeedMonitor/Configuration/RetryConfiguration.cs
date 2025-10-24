namespace Diginsight.Tools.FeedMonitor;

using System;

/// <summary>
/// POCO configuration class for Azure SDK ClientOptions Retry settings.
/// Used to configure retry behavior for Azure SDK clients.
/// </summary>
public class RetryConfiguration
{
    /// <summary>
    /// Gets or sets the maximum number of retry attempts.
    /// Default: 3
    /// </summary>
    public int? MaxRetries { get; set; } = 3;
    
    /// <summary>
    /// Gets or sets the retry mode.
    /// Valid values: "Exponential", "Fixed"
    /// Default: "Exponential"
    /// </summary>
  public string? Mode { get; set; } // e.g., "Exponential", "Fixed"
    
    /// <summary>
    /// Gets or sets the delay between retry attempts in seconds.
    /// For Fixed mode: constant delay between retries.
    /// For Exponential mode: initial delay before exponential backoff.
    /// </summary>
    public int? DelaySeconds { get; set; }
    
    /// <summary>
    /// Gets or sets the maximum delay between retry attempts in seconds.
    /// Applies to Exponential mode to cap the maximum delay.
    /// </summary>
    public int? MaxDelaySeconds { get; set; }
    
    /// <summary>
    /// Gets or sets the network timeout for each request in seconds.
    /// This is the timeout for a single request attempt, not the total retry time.
    /// </summary>
    public int? NetworkTimeoutSeconds { get; set; }
}
