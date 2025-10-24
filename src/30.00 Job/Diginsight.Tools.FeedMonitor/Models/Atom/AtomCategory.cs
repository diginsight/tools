namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// Atom category construct
/// </summary>
public class AtomCategory
{
    /// <summary>
    /// Category identifier (required)
    /// </summary>
    public string Term { get; set; }

    /// <summary>
    /// Categorization scheme IRI
    /// </summary>
    public string Scheme { get; set; }

    /// <summary>
    /// Human-readable label
    /// </summary>
    public string Label { get; set; }

    public override string ToString() => Label ?? Term;
}
