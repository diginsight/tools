using Diginsight.AspNetCore;
using Diginsight.Components.Configuration;
using Diginsight.Diagnostics;
using Diginsight.Stringify;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace Diginsight.Tools.FeedMonitor;

public static class ObservabilityExtensions
{
    static Type T = typeof(ObservabilityExtensions);

    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        EarlyLoggingManager observabilityManager,
        IConfiguration configuration,
        IHostEnvironment hostEnvironment
    )
    {
        var logger = observabilityManager.LoggerFactory.CreateLogger<Program>();
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { services });

        services.AddAspNetCoreObservability(configuration, hostEnvironment, out IOpenTelemetryOptions openTelemetryOptions);
        observabilityManager.AttachTo(services);

        services.AddHttpObservability(openTelemetryOptions);

        services.AddHttpContextAccessor();
        services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

        if (openTelemetryOptions.EnableTraces)
        {
            services.AddDiginsightOpenTelemetry()
                    .WithTracing(
                    static tracerProviderBuilder =>
                    {
                        tracerProviderBuilder.AddHttpClientInstrumentation(
                            static options =>
                            {
                                var enrichWithHttpRequestMessage = options.EnrichWithHttpRequestMessage;
                                options.EnrichWithHttpRequestMessage = (activity, httpRequestMessage) =>
                                {
                                    enrichWithHttpRequestMessage?.Invoke(activity, httpRequestMessage);

                                    if (activity?.FindLabeledParent("Controller")?.GetTagItem("widget_template") is { } widgetTemplateName)
                                    {
                                        activity.SetTag("widget_template", widgetTemplateName);
                                    }
                                };
                            }
                        );
                    }
                );
        }

        services.AddDiginsightOpenTelemetry().WithTracing(b => b.SetSampler(new AlwaysOnSampler()));
        // services.TryAddEnumerable(ServiceDescriptor.Singleton<IActivityListenerRegistration, ControllerActivityTaggerRegistration>());

        services.AddDynamicLogLevel<DefaultDynamicLogLevelInjector>();


        // configure type contracts for log string rendering
        static void ConfigureTypeContracts(StringifyTypeContractAccessor accessor)
        {
            //accessor.GetOrAdd<RestResponse>(
            //    static typeContract =>
            //    {
            //        typeContract.GetOrAdd(static x => x.Request, static mc => mc.Included = false);
            //        typeContract.GetOrAdd(static x => x.ResponseStatus, static mc => mc.Order = 1);
            //        //typeContract.GetOrAdd(static x => x.Content, static mc => mc.Order = 1);
            //    }
            //);
        }
        StringifyContextFactoryBuilder.DefaultBuilder.ConfigureContracts(ConfigureTypeContracts);
        services.Configure<StringifyTypeContractAccessor>(ConfigureTypeContracts);

        //services
        //.AddAnalysis()
        //.Configure<AnalysisOptions>(configuration.GetSection("Observability:Analysis"));

        return services;
    }
}
