using Diginsight;
using Diginsight.Diagnostics;
using Diginsight.Diagnostics.Log4Net;
using log4net.Appender;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CosmosdbConsole;
public static partial class ObservabilityExtensions
{
    static Type T = typeof(ObservabilityExtensions);

    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment hostEnvironment
    )
    {
        bool isLocal = hostEnvironment.IsDevelopment(); 
        string assemblyName = Assembly.GetEntryAssembly()!.GetName().Name!; 

        services.ConfigureClassAware<DiginsightActivitiesOptions>(configuration.GetSection("Diginsight:Activities"));
        services.Configure<DiginsightConsoleFormatterOptions>(configuration.GetSection("Diginsight:Console"));
        services.AddLogging(
                 loggingBuilder =>
                 {
                     loggingBuilder.ClearProviders();

                     if (configuration.GetValue("Observability:ConsoleEnabled", true))
                     {
                         loggingBuilder.AddDiginsightConsole();
                     }

                     if (configuration.GetValue("Observability:Log4NetEnabled", true))
                     {
                         //loggingBuilder.AddDiginsightLog4Net("log4net.config");
                         loggingBuilder.AddDiginsightLog4Net(static sp =>
                         {
                             IHostEnvironment env = sp.GetRequiredService<IHostEnvironment>();
                             string fileBaseDir = env.IsDevelopment()
                                     ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.DoNotVerify)
                                     : $"{Path.DirectorySeparatorChar}home";

                             return new IAppender[]
                             {
                                       new RollingFileAppender()
                                       {
                                           File = Path.Combine(fileBaseDir, "LogFiles", "Diginsight", typeof(Program).Namespace!),
                                           AppendToFile = true,
                                           StaticLogFileName = false,
                                           RollingStyle = RollingFileAppender.RollingMode.Composite,
                                           DatePattern = @".yyyyMMdd.\l\o\g",
                                           MaxSizeRollBackups = 1000,
                                           MaximumFileSize = "100MB",
                                           LockingModel = new FileAppender.MinimalLock(),
                                           Layout = new DiginsightLayout()
                                           {
                                               Pattern = "{Timestamp} {Category} {LogLevel} {TraceId} {Delta} {Duration} {Depth} {Indentation|-1} {Message}",
                                           },
                                       },
                             };
                         },
                         static _ => log4net.Core.Level.All);
                     }
                 });
        services.TryAddSingleton<IActivityLoggingSampler, NameBasedActivityLoggingSampler>();

        return services;
    }

}
