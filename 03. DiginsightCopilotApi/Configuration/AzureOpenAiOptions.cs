using Diginsight.Options;

namespace DiginsightCopilotApi.Configuration;

public class AzureOpenAiOptions : IDynamicallyConfigurable, IVolatilelyConfigurable
{
    public string ApiKey { get; set; }
    public string Endpoint { get; set; }
    public string ChatModel { get; set; }
}