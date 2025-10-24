namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// Atom person construct (author, contributor)
/// </summary>
public class AtomPerson
{
    /// <summary>
    /// Person's name (required)
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// IRI associated with person (homepage, profile)
    /// </summary>
    public string Uri { get; set; }

    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; set; }

    public override string ToString() => Name;
}
