namespace Diginsight.Tools.FeedMonitor;

using System;

public class CosmosDBOptions
{
    public Uri EndpointUri { get; set; }

    public string AuthKey { get; set; }

    public string ConnectionString { get; set; }

    public string Database { get; set; }

    public string Collection { get; set; }

    public string PartitionKey { get; set; }
}
