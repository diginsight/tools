namespace Diginsight.Tools.FeedMonitor;

public class TableClientConfiguration : ClientConfiguration
{
    public string TableName { get; set; }

    // Authentication options - use one of these methods:

    // Method 1: Connection String (simplest)
    public string? ConnectionString { get; set; }

    // Method 2: Endpoint URI with credentials
    /// <summary>
    /// Azure Table Storage endpoint URI.
    /// Used with TokenCredential, AzureSasCredential, or TableSharedKeyCredential.
    /// Example: "https://myaccount.table.core.windows.net"
    /// </summary>
    public string? EndpointUri { get; set; }

    // Method 3: Shared Access Signature (SAS) token
    /// <summary>
    /// SAS token for authentication.
    /// Used with EndpointUri to create AzureSasCredential.
    /// Example: "sv=2019-02-02&ss=t&srt=sco&sp=rwdlacu&se=..."
    /// </summary>
    public string? SasToken { get; set; }

    // Method 4: Shared Key credentials
    /// <summary>
    /// Storage account name for TableSharedKeyCredential authentication.
    /// Used with AccountKey to create TableSharedKeyCredential.
    /// </summary>
    public string? AccountName { get; set; }

    /// <summary>
    /// Storage account key for TableSharedKeyCredential authentication.
    /// Used with AccountName to create TableSharedKeyCredential.
    /// Base64-encoded account key.
    /// </summary>
    public string? AccountKey { get; set; }

    // Method 5: Azure AD / Managed Identity (TokenCredential)
    /// <summary>
    /// Indicates whether to use DefaultAzureCredential for authentication.
    /// When true, uses Azure AD authentication (managed identity, Azure CLI, etc.)
    /// Requires EndpointUri to be set.
    /// </summary>
    public bool UseDefaultAzureCredential { get; set; }

    // TableClientOptions-specific properties
    public string? Audience { get; set; }
    public bool EnableTenantDiscovery { get; set; }
}
