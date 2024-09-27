using Diginsight.AspNetCore;
using Diginsight.Diagnostics.Log4Net;
using Diginsight.Diagnostics;
using Diginsight;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using log4net.Appender;
using log4net.Core;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace DiginsightCopilotApi.Configuration;

public static class AddObservabilityExtension
{
    public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration)
    {
        var logger = Program.LoggerFactory.CreateLogger(typeof(AddObservabilityExtension));
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { services, configuration });

        services.AddHttpContextAccessor();
        services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
        //services.TryAddEnumerable(ServiceDescriptor.Singleton<IActivityListenerRegistration, ActivitySourceDetectorRegistration>());

        services.AddLogging(
            loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
                loggingBuilder.ClearProviders();

                if (configuration.GetValue("AppSettings:ConsoleProviderEnabled", true))
                {
                    loggingBuilder.AddDiginsightConsole(configuration.GetSection("Diginsight:Console").Bind);
                }

                if (configuration.GetValue("AppSettings:Log4NetProviderEnabled", false))
                {
                    loggingBuilder.AddDiginsightLog4Net(
                        static sp =>
                        {
                            IHostEnvironment env = sp.GetRequiredService<IHostEnvironment>();
                            string fileBaseDir = env.IsDevelopment()
                                ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile, Environment.SpecialFolderOption.DoNotVerify)
                                : $"{Path.DirectorySeparatorChar}home";

                            var fileName = typeof(Program).Namespace ?? Assembly.GetEntryAssembly().GetName().Name;

                            return new IAppender[]
                            {
                                new RollingFileAppender()
                                {
                                    File = Path.Combine(fileBaseDir, "LogFiles", "Diginsight", fileName),
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
                        static _ => Level.All
                    );
                }
            }
        );

        services.ConfigureClassAware<DiginsightActivitiesOptions>(configuration.GetSection("Diginsight:Activities"))
                .DynamicallyConfigureClassAwareFromHttpRequestHeaders<DiginsightActivitiesOptions>();

        services.AddDynamicLogLevel<DefaultDynamicLogLevelInjector>();

        return services;
    }

}
