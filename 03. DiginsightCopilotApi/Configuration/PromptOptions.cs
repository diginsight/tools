using Diginsight.Options;

namespace DiginsightCopilotApi.Configuration;

public class PromptOptions : IDynamicallyConfigurable, IVolatilelyConfigurable
{
    public string BlobStorageConnectionString { get; set; }
    public string StorageAccount { get; set; }
    public string Container { get; set; }
    public string PromptFolder { get; set; }
    public string TemplateFolder { get; set; }
}