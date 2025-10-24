namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// Represents a media enclosure (podcast audio, video, etc.)
/// </summary>
public class MediaEnclosure
{
    /// <summary>
    /// Direct URL to the media file
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// MIME type (e.g., "audio/mpeg", "video/mp4")
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long? Length { get; set; }

    /// <summary>
    /// Media duration (for audio/video)
    /// </summary>
    public TimeSpan? Duration { get; set; }
}
