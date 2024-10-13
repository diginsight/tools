namespace DiginsightCopilotApi.Configuration;

public class AzureResourcesOptions
{
    public string AzureMonitorConnectionString { get; set; }
    public string InstrumentationKey { get; set; }
    public string IngestionEndpoint { get; set; }
    public string LiveEndpoint { get; set; }
    public string ApplicationId { get; set; }
    public string ApplicationInsightName { get; set; }
    public string ApplicationInsightResourceGroup { get; set; }
    public string SubscriptionId { get; set; }
}