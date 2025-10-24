namespace Diginsight.Tools.FeedMonitor;

using System;

/// <summary>
/// POCO configuration class for Azure SDK ClientOptions Diagnostics settings.
/// Used to configure logging and tracing behavior for Azure SDK clients.
/// </summary>
public class DiagnosticsConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether logging is enabled.
    /// Default: true
    /// </summary>
    public bool IsLoggingEnabled { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether distributed tracing is enabled.
    /// Default: true
    /// </summary>
    public bool IsDistributedTracingEnabled { get; set; } = true;
    
    /// <summary>
    /// Gets or sets a value indicating whether content logging is enabled.
    /// When true, request and response content will be logged (may contain sensitive data).
    /// Default: false
    /// </summary>
    public bool IsLoggingContentEnabled { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the maximum number of header names to log.
    /// Null means all headers will be logged (subject to allow/deny lists).
    /// </summary>
    public int? LoggedHeaderNamesCount { get; set; }
 
    /// <summary>
    /// Gets or sets the maximum number of query parameters to log.
    /// Null means all query parameters will be logged (subject to allow/deny lists).
    /// </summary>
    public int? LoggedQueryParametersCount { get; set; }
    
    /// <summary>
    /// Gets or sets whether telemetry is enabled.
 /// Controls whether Azure SDK sends telemetry about SDK usage.
    /// </summary>
    public bool? IsTelemetryEnabled { get; set; }

    /// <summary>
    /// Gets or sets the application ID to use in telemetry and user agent strings.
    /// </summary>
    public string? ApplicationId { get; set; }
}
