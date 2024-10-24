using Diginsight.Options;

namespace DiginsightCopilotApi.Configuration;

public class BlobStorageOptions : IDynamicallyConfigurable, IVolatilelyConfigurable
{
    public string BlobStorageConnectionString { get; set; }
    public string StorageAccount { get; set; }
    public string Container { get; set; }
}