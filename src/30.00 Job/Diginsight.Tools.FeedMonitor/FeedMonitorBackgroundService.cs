using ABB.EL.Common.WebJobs.ResourceMonitor.Configuration;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using Diginsight.Components;
using Diginsight.Components.Configuration;
using Diginsight.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace Diginsight.Tools.FeedMonitor;

public class FeedMonitorBackgroundService : BackgroundService
{
    private readonly IOptionsMonitor<FeedMonitorOptions> resourceMonitorOptions;
    private readonly ILogger<FeedMonitorBackgroundService> logger;
    private readonly IConfiguration congiguration;
    private readonly IHostEnvironment environment;
    private readonly IParallelService parallelService;
    private readonly TimeProvider timeProvider;
    private readonly IHostApplicationLifetime applicationLifetime;
    //private readonly IMonitoringRepository monitoringRepository;

    public FeedMonitorBackgroundService(
            ILogger<FeedMonitorBackgroundService> logger,
            IHostEnvironment environment,
            IConfiguration configuration,
            IOptionsMonitor<FeedMonitorOptions> resourceMonitorOptions,
            IParallelService parallelService,
            TimeProvider timeProvider,
            //IMonitoringRepository monitoringRepository,
            IHostApplicationLifetime applicationLifetime)
    {
        this.logger = logger;
        this.congiguration = configuration;
        this.environment = environment;
        this.resourceMonitorOptions = resourceMonitorOptions;
        this.parallelService = parallelService;
        this.timeProvider = timeProvider;
        //this.monitoringRepository = monitoringRepository;
        this.applicationLifetime = applicationLifetime;
    }
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger);

        // Collection of async operations to execute based on configuration
        List<Func<CancellationToken, Task>> asyncInvocations = new();

        asyncInvocations.Add(async ct => { await ReadAllResourcesMetricsAsync(ct); });

        try
        {
            // Execute all configured processing modes concurrently
            await Task.WhenAll(asyncInvocations.Select(f => f(cancellationToken)));
        }
        catch (OperationCanceledException)
        {
            // Expected exception during graceful shutdown - no action needed
        }

        // Signal application shutdown after all processing is complete
        // This ensures the WebJob terminates cleanly
        applicationLifetime.StopApplication();
    }

    private async Task ReadAllResourcesMetricsAsync(CancellationToken cancellationToken)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger);

        int processedCount = 0;
        var allMetricValues = new List<MetricValue>();

        try
        {
            logger.LogInformation("Starting one-time plant license activation processing");

            var optionsValues = resourceMonitorOptions.CurrentValue;

            var credentialProvider = new DefaultCredentialProvider(environment);
            var credential = credentialProvider.Get(congiguration.GetSection("ResourceMonitor"));
            var client = new MetricsQueryClient(credential);

            // Process each configured resource
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = parallelService.MediumConcurrency };
            await parallelService.ForEachAsync<FeedItem>(optionsValues.Feeds, parallelOptions, async (feedConfig) =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var utcNow = timeProvider.GetUtcNow();
                string feedUri = feedConfig.Uri;

                //    if (string.IsNullOrEmpty(resourceId) || metrics == null || metrics.Length == 0) { logger.LogWarning("Skipping resource with empty resourceId or metrics"); return; }
                logger.LogInformation("Querying feed: {feedUri} at: {utcNow}", feedUri, utcNow);

                // var feedData = await feedConfig.FetchFeedAsync(logger, cancellationToken);




            }).ConfigureAwait(false);


            //    var metricsQueryOptions = new MetricsQueryOptions
            //    {
            //        TimeRange = interval,
            //        Granularity = granularity
            //    };
            //    metricsQueryOptions.Aggregations.AddRange(aggregations);

            //    Response<MetricsQueryResult> response = await client.QueryResourceAsync(resourceId, metrics, metricsQueryOptions, cancellationToken);

            //    var resourceMetricValues = new List<MetricValue>();
            //    foreach (var metric in response.Value.Metrics)
            //    {
            //        logger.LogDebug("Metric: {MetricName} for Resource: {ResourceId}", metric.Name, resourceId);

            //        foreach (var timeSeries in metric.TimeSeries)
            //        {
            //            foreach (var data in timeSeries.Values)
            //            {
            //                logger.LogDebug("Timestamp: {Timestamp}, Average: {Average}, Count: {Count}, Maximum: {Maximum}, Minimum: {Minimum}, Total: {Total}", data.TimeStamp, data.Average, data.Count, data.Maximum, data.Minimum, data.Total);
            //                var metricValue = new MetricValue
            //                {
            //                    Id = Guid.NewGuid(),
            //                    Type = typeof(MetricValue).Name,
            //                    ResourceId = resourceId,
            //                    Name = metric.Name,
            //                    ResourceType = ExtractResourceTypeFromResourceId(resourceId),
            //                    Timestamp = data.TimeStamp,
            //                    Average = data.Average,
            //                    Count = data.Count,
            //                    Maximum = data.Maximum,
            //                    Minimum = data.Minimum,
            //                    Total = data.Total,
            //                    ttl = optionsValues.TimeToLive
            //                };
            //                resourceMetricValues.Add(metricValue);
            //            }
            //        }
            //    }
            //    metricValues.AddRange(resourceMetricValues.OrderByDescending(m => m.Timestamp));

            //    if (metricValues.Any())
            //    {
            //        var loadExistingMetrics = optionsValues.LoadExistingMetrics;
            //        if (loadExistingMetrics)
            //        {
            //            var runsSchedulingInMinutes = optionsValues.RunsSchedulingInMinutes;
            //            var loadExistingMetricsIntervalMinimumInMinutes = optionsValues.LoadExistingMetricsIntervalMinimumInMinutes;
            //            var metricsInterval = Math.Max(runsSchedulingInMinutes * 2, loadExistingMetricsIntervalMinimumInMinutes);
            //            var exisingMetricsIntervalStart = DateTimeOffset.UtcNow.AddMinutes(-metricsInterval);
            //            var oldestMetricTimestamp = metricValues.LastOrDefault()?.Timestamp;
            //            if (oldestMetricTimestamp != null && oldestMetricTimestamp.Value < exisingMetricsIntervalStart) { exisingMetricsIntervalStart = oldestMetricTimestamp.Value; }


            //            var recordedMetrics = await monitoringRepository.QueryDocumentsAsync(
            //                documents => documents.Where(document => document.Type == typeof(MetricValue).Name &&
            //                                                         document.ResourceId == resourceId &&
            //                                                         document.Timestamp >= exisingMetricsIntervalStart
            //                                                         )
            //                                      .OrderByDescending(document => document.Timestamp),
            //                cancellationToken: cancellationToken).ConfigureAwait(false);


            //            var existingMetrics = from m in metricValues
            //                                  join r in recordedMetrics on new { m.ResourceId, m.Name, m.Timestamp } equals new { r.ResourceId, r.Name, r.Timestamp }
            //                                  select m;

            //            var existingMetricIds = existingMetrics.Select(m => m.Id).ToHashSet();
            //            if (existingMetricIds.Any())
            //            {
            //                metricValues = metricValues.Where(m => !existingMetricIds.Contains(m.Id))?.OrderByDescending(m => m.Timestamp)?.ToList();
            //            }
            //        }

            //        logger.LogInformation("Writing {MetricCount} metric values to CosmosDB", metricValues.Count);
            //        await monitoringRepository.UpsertDocumentsAsync(metricValues, cancellationToken);
            //        logger.LogInformation("Successfully wrote {MetricCount} metric values to CosmosDB", metricValues.Count);
            //        logger.LogInformation("first metric: {Metric}", metricValues.LastOrDefault().Stringify());
            //        logger.LogInformation("last  metric: {Metric}", metricValues.FirstOrDefault().Stringify());
            //    }
            //    else
            //    {
            //        logger.LogInformation("No metric values to write to CosmosDB");
            //    }

            //    logger.LogInformation("One-time processing completed. {ProcessedCount} resources processed, {MetricCount} metric values collected", processedCount, metricValues.Count);

            //    lock (allMetricValues) { allMetricValues.AddRange(metricValues); }

            //    Interlocked.Increment(ref processedCount);
            //    return;
            //}).ConfigureAwait(false);

        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Log any unexpected errors but don't rethrow to allow graceful shutdown
            logger.LogError(ex, "Error occurred during one-time plant license activation processing");
        }

        // Set the processed count as activity output for observability tracking
        activity?.SetOutput(new { processedCount, metricCount = allMetricValues.Count });
    }

}
