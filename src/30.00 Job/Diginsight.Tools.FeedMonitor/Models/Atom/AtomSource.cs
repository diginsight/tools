using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diginsight.Tools.FeedMonitor;

/// <summary>
/// Atom source metadata for aggregated entries
/// </summary>
public class AtomSource
{
    public string Id { get; set; }
    public string Title { get; set; }
    public DateTime? Updated { get; set; }
    public List<AtomLink> Links { get; set; } = new List<AtomLink>();
}