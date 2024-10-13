namespace DiginsightCopilotApi.Configuration;

public class PromptOptions
{
    public string BlobStorageConnectionString { get; set; }
    public string StorageAccount { get; set; }
    public string Container { get; set; }
    public string PromptFolder { get; set; }
}