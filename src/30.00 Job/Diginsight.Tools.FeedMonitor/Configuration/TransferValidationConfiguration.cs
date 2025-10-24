namespace Diginsight.Tools.FeedMonitor;

using System;

/// <summary>
/// POCO configuration class for Azure Storage TransferValidationOptions.
/// Used to configure data integrity validation during upload/download operations.
/// </summary>
public class TransferValidationConfiguration
{
    /// <summary>
    /// Checksum algorithm to use for data validation.
    /// Valid values: "MD5", "StorageCrc64", "Auto", "None"
  /// </summary>
    public string? ChecksumAlgorithm { get; set; } // e.g., "MD5", "StorageCrc64"
    
    /// <summary>
    /// Pre-calculated checksum value (optional).
    /// If provided, it will be used instead of calculating a new one.
    /// Format depends on ChecksumAlgorithm (e.g., Base64 for MD5)
    /// </summary>
    public string? PrecalculatedChecksum { get; set; }
}
