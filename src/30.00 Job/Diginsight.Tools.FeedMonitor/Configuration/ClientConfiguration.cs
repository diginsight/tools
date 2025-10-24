namespace Diginsight.Tools.FeedMonitor;

using System;

/// <summary>
/// Base POCO class for Azure SDK ClientOptions configuration.
/// Contains common properties that can be used to initialize various Azure SDK client options.
/// </summary>
public abstract class ClientConfiguration
{
    public bool Enabled { get; set; }

    public string? ServiceVersion { get; set; } // e.g., "V2020_10_02", "V2021_12_02"

    // Retry configuration
    public RetryConfiguration? Retry { get; set; }
    
    // Diagnostics configuration
    public DiagnosticsConfiguration? Diagnostics { get; set; }

    // Transport configuration
    // Note: Transport (HttpPipelineTransport) is too complex for direct POCO configuration
    // It should be configured programmatically when building ClientOptions
    // You can add flags here to indicate which transport to use
    public string? TransportType { get; set; } // e.g., "Default", "HttpClient" - for future extensibility
}
