namespace Diginsight.Tools.FeedMonitor;

using System;

/// <summary>
/// POCO configuration class for Azure Storage CustomerProvidedKey.
/// Used to configure customer-managed encryption keys for blob storage operations.
/// </summary>
public class CustomerProvidedKeyConfiguration
{
    /// <summary>
    /// Base64-encoded encryption key.
    /// Must be a valid AES-256 key (32 bytes before encoding).
    /// </summary>
    public string? EncryptionKey { get; set; }
    
    /// <summary>
    /// Base64-encoded SHA256 hash of the encryption key.
    /// This is used by Azure Storage to verify the key integrity.
    /// </summary>
    public string? EncryptionKeyHash { get; set; }
    
/// <summary>
    /// Encryption algorithm to use.
    /// Valid values: "AES256" (default and currently the only supported algorithm)
    /// </summary>
    public string? EncryptionAlgorithm { get; set; } = "AES256";
}
