namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// iTunes category structure
/// </summary>
public class ItunesCategory
{
    public string Text { get; set; }
    public ItunesCategory Subcategory { get; set; }
}
