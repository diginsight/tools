using Diginsight.Diagnostics;
using System.Diagnostics;
using System.Reflection;

namespace Learn.Web;

internal static class Observability
{
    public static readonly ActivitySource ActivitySource = new(Assembly.GetExecutingAssembly().GetName().Name!);
    public static ILoggerFactory? LoggerFactory => LoggerFactoryStaticAccessor.LoggerFactory;
}
