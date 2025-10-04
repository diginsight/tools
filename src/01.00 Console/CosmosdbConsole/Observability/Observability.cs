using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;

namespace CosmosdbConsole;

internal static class Observability
{
    public static readonly ActivitySource ActivitySource = new(Assembly.GetExecutingAssembly().GetName().Name!);
    public static ILoggerFactory LoggerFactory { get; set; }
}

