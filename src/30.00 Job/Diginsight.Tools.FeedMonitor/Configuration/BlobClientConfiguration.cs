namespace Diginsight.Tools.FeedMonitor;

using System;

public class BlobClientConfiguration : ClientConfiguration
{
    public string ContainerName { get; set; }

    // Authentication options - use one of these methods:

    // Method 1: Connection String (simplest)
    public string? ConnectionString { get; set; }


    // Method 2: Endpoint URI with credentials
    /// <summary>
    /// Azure Blob Storage endpoint URI.
    /// Used with TokenCredential, AzureSasCredential, or StorageSharedKeyCredential.
    /// Example: "https://myaccount.blob.core.windows.net"
    /// </summary>
    public string? EndpointUri { get; set; }

    // Method 3: Shared Access Signature (SAS) token
    /// <summary>
    /// SAS token for authentication.
    /// Used with EndpointUri to create AzureSasCredential.
    /// Example: "sv=2019-02-02&ss=b&srt=sco&sp=rwdlacu&se=..."
    /// </summary>
    public string? SasToken { get; set; }

    // Method 4: Shared Key credentials
    /// <summary>
    /// Storage account name for StorageSharedKeyCredential authentication.
    /// Used with AccountKey to create StorageSharedKeyCredential.
    /// </summary>
    public string? AccountName { get; set; }

    /// <summary>
    /// Storage account key for StorageSharedKeyCredential authentication.
    /// Used with AccountName to create StorageSharedKeyCredential.
    /// Base64-encoded account key.
    /// </summary>
    public string? AccountKey { get; set; }

    // Blob-specific options
    public char PathDelimiter { get; set; } = '/';
    public bool EnableSoftDeletion { get; set; }
    public int DeleteRetentionDays { get; set; }
    public TimeSpan SharedAccessSignatureExpiration { get; set; }
    public int? MaxItemCount { get; set; }

    // BlobClientOptions-specific properties
    public string? Audience { get; set; }
    public bool EnableTenantDiscovery { get; set; }
    public string? EncryptionScope { get; set; }
    public string? GeoRedundantSecondaryUri { get; set; }
    public int? Request100ContinueTryTimeoutSeconds { get; set; } = 30;
    public bool TrimBlobNameSlashes { get; set; } = true;

    // Nested configuration objects
    public CustomerProvidedKeyConfiguration? CustomerProvidedKey { get; set; }
    public TransferValidationConfiguration? TransferValidation { get; set; }
}
