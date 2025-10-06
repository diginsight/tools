using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Diginsight.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Diginsight.Tools.FeedMonitor;

internal static class Observability
{
    public static readonly ActivitySource ActivitySource = new(Assembly.GetExecutingAssembly().GetName().Name!);
    public static ILoggerFactory LoggerFactory { get; set; } = null!;
    static Observability() => ObservabilityRegistry.RegisterComponent(factory => LoggerFactory = factory);
}
