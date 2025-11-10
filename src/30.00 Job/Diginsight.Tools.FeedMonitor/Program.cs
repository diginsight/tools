using Diginsight.Components;
using Diginsight.Components.Configuration;
using Diginsight.Diagnostics;
using Microsoft.Azure.Cosmos;
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
        //Console.OutputEncoding = System.Text.Encoding.UTF8;
        //Console.InputEncoding = System.Text.Encoding.UTF8;
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

        services.AddHttpContextAccessor();

        //services.AddAspNetCoreObservability(configuration, hostEnvironment, out IOpenTelemetryOptions openTelemetryOptions);
        services.AddObservability(configuration, hostEnvironment, out IOpenTelemetryOptions openTelemetryOptions);
        observabilityManager.AttachTo(services);

        services.AddParallelService(configuration);

        services.AddHttpClient(Microsoft.Extensions.Options.Options.DefaultName)
                .AddBodyLoggingHandler();

        services.ConfigureClassAware<FeedMonitorConfiguration>(configuration.GetSection("FeedMonitor"))
                .DynamicallyConfigure<FeedMonitorConfiguration>()
                .VolatilelyConfigure<FeedMonitorConfiguration>();

        services.Configure<CosmosDBClientConfiguration>("FeedMonitorCosmosDBOptions", opt =>
        {
            opt.Enabled = configuration.GetValue<bool>("FeedMonitor:CosmosDB:Enabled")!;
            opt.ConnectionString = configuration.GetValue<string>("FeedMonitor:CosmosDB:ConnectionString")!;
            opt.EndpointUri = configuration.GetValue<Uri>("FeedMonitor:CosmosDB:EndpointUri")!;
            opt.AuthKey = configuration.GetValue<string>("FeedMonitor:CosmosDB:AuthKey")!;
            opt.Collection = configuration.GetValue<string>("FeedMonitor:CosmosDB:Collection")!;
            opt.Database = configuration.GetValue<string>("FeedMonitor:CosmosDB:Database")!;
            var connectionModeValue = configuration.GetValue<string>("FeedMonitor:CosmosDB:ConnectionMode");
            if (Enum.TryParse<ConnectionMode>(connectionModeValue, true, out var connectionMode))
            {
                opt.ConnectionMode = connectionMode;
            }
        });

        services.Configure<BlobClientConfiguration>("FeedMonitorBlobStorageOptions", 
            configuration.GetSection("FeedMonitor:BlobStorage"));

        services.Configure<TableClientConfiguration>("FeedMonitorTableStorageOptions", 
            configuration.GetSection("FeedMonitor:TableStorage"));

        services.Configure<FileStorageClientConfiguration>("FeedMonitorFileStorageOptions", opt =>
        {
            opt.ConnectionString = configuration.GetValue<string>("FeedMonitor:FileStorage:ConnectionString")!;
        });

        services.Configure<QueueStorageClientConfiguration>("FeedMonitorQueueStorageOptions", opt =>
        {
            opt.ConnectionString = configuration.GetValue<string>("FeedMonitor:QueueStorage:ConnectionString")!;
        });

        services.TryAddSingleton(TimeProvider.System);

        // Register feed parsing services
        services.AddFeedParsing();

        services.AddHostedService<FeedMonitorBackgroundService>();

        hostBuilder.UseDiginsightServiceProvider();

        var host = hostBuilder.Build();

        await host.RunAsync();
    }
}
