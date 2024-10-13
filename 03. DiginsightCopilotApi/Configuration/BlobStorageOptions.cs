namespace DiginsightCopilotApi.Configuration;

public class BlobStorageOptions
{
    public string BlobStorageConnectionString { get; set; }
    public string StorageAccount { get; set; }
    public string Container { get; set; }
}