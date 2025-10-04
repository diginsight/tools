using Cocona;
using Cocona.Builder;
using CosmosdbConsole;
using Diginsight;
using Diginsight.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CosmosdbConsole;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using var observabilityManager = new ObservabilityManager();
        ILogger logger = observabilityManager.LoggerFactory.CreateLogger(typeof(Program));
        Observability.LoggerFactory = observabilityManager.LoggerFactory;

        AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

        CoconaApp app = default!;
        using (var activity = Observability.ActivitySource.StartMethodActivity(logger, new { args }))
        {
            CoconaAppBuilder appBuilder = CoconaApp.CreateBuilder(args);

            IConfiguration configuration = appBuilder.Configuration;
            IServiceCollection services = appBuilder.Services;
            IHostEnvironment hostEnvironment = appBuilder.Environment;

            services.AddObservability(configuration, hostEnvironment);
            observabilityManager.AttachTo(services);
            services.TryAddSingleton<IActivityLoggingSampler, NameBasedActivityLoggingSampler>();

            services.AddSingleton<Executor>();

            appBuilder.Host.UseDiginsightServiceProvider(true);
            app = appBuilder.Build();


            Executor executor = app.Services.GetRequiredService<Executor>();
            app.AddCommand("loadjson", executor.StreamDocumentsJsonAsync);
            app.AddCommand("query", executor.QueryAsync);
            app.AddCommand("uploadjson", executor.UploadDocumentsJsonAsync);
            app.AddCommand("deletefromjson", executor.DeleteDocumentsFromJsonAsync);
        }

        await app.RunAsync();
    }
}
