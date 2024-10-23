using Diginsight;
using Diginsight.Diagnostics;
using DiginsightCopilotApi;
using DiginsightCopilotApi.Abstractions;
using DiginsightCopilotApi.Configuration;
using DiginsightCopilotApi.Models;
using DiginsightCopilotApi.Services;
using Microsoft.Extensions.Hosting;
using static DiginsightCopilotApi.Services.AOAISummaryService;



public class Program
{
    public static ILoggerFactory LoggerFactory = null; 

    static Program() 
    {
        var activitiesOptions = new DiginsightActivitiesOptions() { LogActivities = true };
        var deferredLoggerFactory = new DeferredLoggerFactory(activitiesOptions: activitiesOptions);
        deferredLoggerFactory.ActivitySourceFilter = (activitySource) => activitySource.Name.StartsWith($"Diginsight");
        LoggerFactory = deferredLoggerFactory;
    }

    static void Main(string[] args)
    {
        var logger = LoggerFactory.CreateLogger(typeof(Program));
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { args });

        var builder = WebApplication.CreateBuilder(args); logger.LogDebug($"var builder = WebApplication.CreateBuilder(args);");

        builder.Host.ConfigureAppConfigurationNH(); logger.LogDebug("builder.Host.ConfigureAppConfigurationNH();");
        builder.Services.AddObservability(builder.Configuration);                               // Diginsight: registers loggers
        builder.Services.FlushOnCreateServiceProvider((IDeferredLoggerFactory)LoggerFactory);   // Diginsight: registers startup log flush
        var webHost = builder.Host.UseDiginsightServiceProvider();                              // Diginsight: Flushes startup log and initializes standard log
        logger.LogDebug("builder.Services.AddObservability(builder.Configuration);");
        logger.LogDebug("builder.Services.FlushOnCreateServiceProvider(deferredLoggerFactory);");
        logger.LogDebug("var webHost = builder.Host.UseDiginsightServiceProvider();");

        builder.Services.AddControllers(); logger.LogDebug($"builder.Services.AddControllers();");
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer(); logger.LogDebug($"builder.Services.AddEndpointsApiExplorer();");
        builder.Services.AddSwaggerGen(); logger.LogDebug($"builder.Services.AddSwaggerGen();");


        builder.Services.Configure<AzureAdOptions>(builder.Configuration.GetSection("AzureAd"));
        builder.Services.Configure<BlobStorageOptions>(builder.Configuration.GetSection("BlobStorage"));
        builder.Services.Configure<PromptOptions>(builder.Configuration.GetSection("Prompt"));
        builder.Services.Configure<AzureOpenAiOptions>(builder.Configuration.GetSection("AzureOpenAi"));
        builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection("Email"));
        
        builder.Services.Configure<AzureDevopsOptions>(builder.Configuration.GetSection("Devops"));
        builder.Services.Configure<HttpContextOptions>(builder.Configuration.GetSection("HttpContext"));
        builder.Services.Configure<AzureResourcesOptions>(builder.Configuration.GetSection("AzureResources"));
        builder.Services.Configure<FeatureFlagOptions>(builder.Configuration.GetSection("AppSettings"));

        builder.Services.AddSingleton<IAzureDevopsService, AzureDevopsService>();
        builder.Services.AddSingleton<ISummaryService, AOAISummaryService>();
        builder.Services.AddSingleton<IEmailService, SmtpMailService>();

        var app = builder.Build(); logger.LogDebug($"var app = builder.Build();");

        // Configure the HTTP request pipeline.
        var isDevelopment = app.Environment.IsDevelopment(); logger.LogDebug("app.Environment.IsDevelopment(); returned {isDevelopment}", isDevelopment);
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(); logger.LogDebug("app.UseSwagger();");
            app.UseSwaggerUI(); logger.LogDebug("app.UseSwaggerUI();");
        }

        app.UseHttpsRedirection(); logger.LogDebug("app.UseHttpsRedirection();");

        app.UseAuthorization(); logger.LogDebug("app.UseAuthorization();");

        app.MapControllers(); logger.LogDebug("app.MapControllers();");

        app.Run(); logger.LogDebug("app.Run();");
    }
}

