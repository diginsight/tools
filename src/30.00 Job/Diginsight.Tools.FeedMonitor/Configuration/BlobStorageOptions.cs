namespace Diginsight.Tools.FeedMonitor;

using System;

public class BlobStorageOptions
{
    public string ConnectionString { get; set; }

    public char PathDelimiter { get; set; } = '/';

    public bool EnableSoftDeletion { get; set; }

    public int DeleteRetentionDays { get; set; }

    public TimeSpan SharedAccessSignatureExpiration { get; internal set; }

    public int? MaxItemCount { get; set; }
}
