
using Diginsight.Diagnostics;
using Diginsight.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
//using Options = Microsoft.Extensions.Options.Options;

namespace Diginsight.Tools.FeedMonitor;

public sealed class ObservabilityManager : EarlyLoggingManager
{
    private IServiceCollection? services;

    static ObservabilityManager()
    {
        _ = Observability.ActivitySource;
    }

    public ObservabilityManager() : base(static activitySource => activitySource == Observability.ActivitySource)
    {
    }

    [SuppressMessage("ReSharper", "ParameterHidesMember")]
    protected override void AdditionalAttachTo(IServiceCollection services)
    {

        this.services = services;
    }

    protected override ILoggerFactory MakeEmergencyLoggerFactory()
    {
        if (services is not { } emergencyServices)
        {
            emergencyServices = new ServiceCollection();

            emergencyServices.Configure<LoggerFilterOptions>(
                static lfo =>
                {
                    lfo.MinLevel = LogLevel.Debug;
                    lfo.Rules.Add(new LoggerFilterRule(null, null, LogLevel.Information, null));
                    lfo.Rules.Add(new LoggerFilterRule(null, "Microsoft.AspNetCore", LogLevel.Warning, null));
                    lfo.Rules.Add(new LoggerFilterRule(null, "*.IdentityLoggerAdapter", LogLevel.Warning, null));
                    lfo.Rules.Add(new LoggerFilterRule(null, "Diginsight", LogLevel.Debug, null));
                    lfo.Rules.Add(new LoggerFilterRule(null, nameof(Program), LogLevel.Trace, null));
                }
            );

            emergencyServices.AddLogging(static lb => { lb.AddDiginsightConsole(); });
        }

        IServiceProvider serviceProvider = emergencyServices.BuildServiceProvider();
        return serviceProvider.GetRequiredService<ILoggerFactory>();
    }

    protected override ActivityLifecycleLogEmitter MakeEmergencyLogEmitter()
    {
        if (services is not null)
        {
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            return ActivatorUtilities.CreateInstance<ActivityLifecycleLogEmitter>(serviceProvider);
        }
        else
        {
            return new ActivityLifecycleLogEmitter(
                GetEmergencyLoggerFactory(),
                new ClassAwareOptionsMonitorExtension<DiginsightActivitiesOptions>(
                    new ClassAwareOptionsExtension<DiginsightActivitiesOptions>(
                        Microsoft.Extensions.Options.Options.Create(new DiginsightActivitiesOptions() { LogBehavior = LogBehavior.Show })
                    )
                )
            );
        }
    }
}
