using Diginsight.Options;

namespace DiginsightCopilotApi.Services;

public class FeatureFlagOptions : IDynamicallyConfigurable, IVolatilelyConfigurable
{
    public bool UseStructuredOutput { get; set; }
}

