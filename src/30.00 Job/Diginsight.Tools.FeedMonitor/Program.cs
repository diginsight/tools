using ABB.EL.Common.WebJobs.ResourceMonitor.Configuration;
using Diginsight.Components;
using Diginsight.Components.Configuration;
using Diginsight.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Diginsight.Tools.FeedMonitor;

internal class Program
{
    private static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.UTF8;
        AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

        using var observabilityManager = new ObservabilityManager();
        ILogger logger = observabilityManager.LoggerFactory.CreateLogger(typeof(Program));
        ObservabilityRegistry.RegisterLoggerFactory(observabilityManager.LoggerFactory);

        using var activity = Observability.ActivitySource.StartMethodActivity(logger);

        HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder();

        hostBuilder.ConfigureAppConfiguration2(observabilityManager.LoggerFactory);

        IHostEnvironment hostEnvironment = hostBuilder.Environment;
        IConfiguration configuration = hostBuilder.Configuration;
        IServiceCollection services = hostBuilder.Services;

        services.AddObservability(configuration, hostEnvironment, out IOpenTelemetryOptions openTelemetryOptions);
        observabilityManager.AttachTo(services);

        services.AddParallelService(configuration);

        services.AddHttpClient(Microsoft.Extensions.Options.Options.DefaultName)
                .AddBodyLoggingHandler();

        services.ConfigureClassAware<FeedMonitorOptions>(configuration.GetSection("FeedMonitor"))
                .DynamicallyConfigure<FeedMonitorOptions>()
                .VolatilelyConfigure<FeedMonitorOptions>();

        //services.Configure<CosmosDBConfiguration>("MonitoringRepoConfig", opt =>
        //{
        //    opt.AuthKey = configuration.GetValue<string>("Monitoring:CosmosDB:AuthKey")!;
        //    opt.Collection = configuration.GetValue<string>("Monitoring:CosmosDB:Collection")!;
        //    opt.Database = configuration.GetValue<string>("Monitoring:CosmosDB:Database")!;
        //    opt.EndpointUri = configuration.GetValue<Uri>("Monitoring:CosmosDB:EndpointUri")!;
        //    opt.LogMetrics = configuration.GetValue<bool>("Monitoring:CosmosDB:LogMetrics");
        //    opt.MaxRequestsPerTcpConnection = configuration.GetValue<int>("Monitoring:CosmosDB:MaxRequestsPerTcpConnection");
        //    opt.MaxRetryAttemptsOnThrottledRequests = configuration.GetValue<int>("Monitoring:CosmosDB:MaxRetryAttemptsOnThrottledRequests");
        //    opt.MaxRetryWaitTimeInSeconds = configuration.GetValue<int>("Monitoring:CosmosDB:MaxRetryWaitTimeInSeconds");
        //    opt.RequestTimeout = configuration.GetValue<TimeSpan>("Monitoring:CosmosDB:RequestTimeout");
        //});

        services.TryAddSingleton(TimeProvider.System);

        services.AddHostedService<FeedMonitorBackgroundService>();

        hostBuilder.UseDiginsightServiceProvider();

        var host = hostBuilder.Build();

        await host.RunAsync();
    }
}
