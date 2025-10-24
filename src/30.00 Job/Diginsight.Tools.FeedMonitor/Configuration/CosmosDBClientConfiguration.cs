namespace Diginsight.Tools.FeedMonitor;

using System;
using Microsoft.Azure.Cosmos;

public class CosmosDBClientConfiguration
{
    public string ConnectionString { get; set; }

    public Uri EndpointUri { get; set; }

    public string Database { get; set; }

    public string Collection { get; set; }

    public string AuthKey { get; set; }

    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(60);

    public bool LogMetrics { get; set; } = false;

    public int MaxRequestsPerTcpConnection { get; set; } = 10;

    public int MaxRetryAttemptsOnThrottledRequests { get; set; } = 9;

    public int MaxRetryWaitTimeInSeconds { get; set; } = 30;

    public ConnectionMode ConnectionMode { get; set; } = ConnectionMode.Direct;

    public string PartitionKey { get; set; }
    public bool Enabled { get; set; }
}
