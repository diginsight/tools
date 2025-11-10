using Azure.Core.Pipeline;
using Azure.Data.Tables;
using Azure.Identity;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Diginsight.Components;
using Diginsight.Components.Azure;
using Diginsight.Components.Configuration;
using Diginsight.Diagnostics;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Container = Microsoft.Azure.Cosmos.Container;


namespace Diginsight.Tools.FeedMonitor;

public class FeedMonitorBackgroundService : BackgroundService
{
    private readonly CosmosClient cosmosClient;
    private readonly Container container;
    private readonly TableServiceClient tableServiceClient;
    private readonly TableClient tableClient;
    private readonly BlobServiceClient blobServiceClient;
    private readonly BlobContainerClient blobContainerClient;

    private readonly IOptionsMonitor<FeedMonitorConfiguration> feedMonitorOptionsMonitor;
    private readonly IOptionsMonitor<CosmosDBClientConfiguration> cosmosDBOptionsMonitor;
    private readonly IOptionsMonitor<TableClientConfiguration> tableStorageOptionsMonitor;

    private readonly IOptionsMonitor<BlobClientConfiguration> blobStorageOptionsMonitor;
    private readonly IOptionsMonitor<FileStorageClientConfiguration> fileStorageOptionsMonitor;
    private readonly IOptionsMonitor<QueueStorageClientConfiguration> queueStorageOptionsMonitor;
    private readonly ILogger<FeedMonitorBackgroundService> logger;
    private readonly IConfiguration configuration;
    private readonly IHostEnvironment environment;
    private readonly IParallelService parallelService;
    private readonly TimeProvider timeProvider;
    private readonly IHostApplicationLifetime applicationLifetime;
    private readonly IFeedParserFactory feedParserFactory;
    //private readonly IMonitoringRepository monitoringRepository;

    public FeedMonitorBackgroundService(
            ILogger<FeedMonitorBackgroundService> logger,
            IOptionsMonitor<FeedMonitorConfiguration> feedMonitorOptionsMonitor,
            IOptionsMonitor<CosmosDBClientConfiguration> cosmosDBOptionsMonitor,
            IOptionsMonitor<BlobClientConfiguration> blobStorageOptionsMonitor,
            IOptionsMonitor<TableClientConfiguration> tableStorageOptionsMonitor,
            IOptionsMonitor<FileStorageClientConfiguration> fileStorageOptionsMonitor,
            IOptionsMonitor<QueueStorageClientConfiguration> queueStorageOptionsMonitor,
            IHostEnvironment environment,
            IConfiguration configuration,
            IParallelService parallelService,
            TimeProvider timeProvider,
            IHostApplicationLifetime applicationLifetime,
            IFeedParserFactory feedParserFactory)
    {
        this.logger = logger;
        this.environment = environment;

        this.feedMonitorOptionsMonitor = feedMonitorOptionsMonitor;
        this.cosmosDBOptionsMonitor = cosmosDBOptionsMonitor;
        this.blobStorageOptionsMonitor = blobStorageOptionsMonitor;
        this.tableStorageOptionsMonitor = tableStorageOptionsMonitor;
        this.fileStorageOptionsMonitor = fileStorageOptionsMonitor;
        this.queueStorageOptionsMonitor = queueStorageOptionsMonitor;
        this.configuration = configuration;

        var cosmosDBClientConfiguration = cosmosDBOptionsMonitor.Get("FeedMonitorCosmosDBOptions");
        if (cosmosDBClientConfiguration.Enabled)
        {
            cosmosClient = GetCosmosClient(cosmosDBClientConfiguration);
            container = cosmosClient.GetContainer(cosmosDBClientConfiguration.Database, cosmosDBClientConfiguration.Collection);
        }

        var tableClientConfiguration = tableStorageOptionsMonitor.Get("FeedMonitorTableStorageOptions");
        if (tableClientConfiguration.Enabled)
        {
            tableServiceClient = GetTableServiceClient(tableClientConfiguration);
            tableClient = tableServiceClient.GetTableClient(tableClientConfiguration.TableName);
        }

        var blobClientConfiguration = blobStorageOptionsMonitor.Get("FeedMonitorBlobStorageOptions");
        if (blobClientConfiguration.Enabled)
        {
            this.blobServiceClient = GetBlobServiceClient(blobClientConfiguration);
            blobContainerClient = blobServiceClient.GetBlobContainerClient(blobClientConfiguration.ContainerName);
        }

        this.parallelService = parallelService;
        this.timeProvider = timeProvider;
        this.feedParserFactory = feedParserFactory;
        // this.monitoringRepository = monitoringRepository;
        this.applicationLifetime = applicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger);

        // Collection of async operations to execute based on configuration
        List<Func<CancellationToken, Task>> asyncInvocations = new();

        asyncInvocations.Add(async ct => { await ReadAllFeedsAsync(ct); });

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

    private async Task ReadAllFeedsAsync(CancellationToken cancellationToken)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger);

        int processedCount = 0;
        var allMetricValues = new List<MetricValue>();

        try
        {
            var feedMonitorOptions = feedMonitorOptionsMonitor.CurrentValue;

            var credentialProvider = new DefaultCredentialProvider(environment);
            var credential = credentialProvider.Get(configuration.GetSection("FeedMonitor"));

            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = parallelService.MediumConcurrency, CancellationToken = cancellationToken };
            await parallelService.ForEachAsync<FeedItem>(feedMonitorOptions.Feeds, parallelOptions, async (feedConfig) =>
            {
                using var inneractivity = Observability.ActivitySource.StartRichActivity(logger, "ReadAllFeedsAsync.FeedIteration", () => new { feedConfig.Uri });
                inneractivity?.SetTag("feed_uri", feedConfig.Uri);

                cancellationToken.ThrowIfCancellationRequested();

                var utcNow = timeProvider.GetUtcNow();
                string feedUri = feedConfig.Uri;
                logger.LogInformation("Querying feed: {feedUri} at: {utcNow}", feedUri, utcNow);

                var xmlContent = default(string?);
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "PodcastFeedParser/1.0 (Compatible RSS Reader)");
                    xmlContent = await httpClient.GetStringAsync(feedUri);
                }

                FeedChannelBase? feedChannel = null;
                try
                {
                    feedChannel = feedParserFactory.ParseFeed(xmlContent);

                    // Set feed channel tracking properties
                    feedChannel.Uri = feedUri;
                    feedChannel.CategoryPath = feedConfig.CategoryPath;

                    // Set partition key based on domain
                    var feedDomain = new Uri(feedUri).Host;
                    feedChannel.PartitionKey = "/";

                    // Use the channel link or URI as the unique ID - sanitize for CosmosDB
                    var rawId = feedChannel.Link ?? feedUri;
                    feedChannel.Id = CosmosDbHelper.SanitizeCosmosDbId(rawId);

                    logger.LogDebug($"Feed: {feedChannel.Title}, Description: {feedChannel.Description}");
                    logger.LogDebug($"Items: {feedChannel.Items.Count}, Feed Type: {feedChannel.FeedType}");
                    if (feedChannel is RSSFeedChannel rssFeed)
                    {
                        logger.LogDebug($"WebSubHub: {rssFeed.WebSubHub ?? "Not available"}, Last Updated: {feedChannel.LastUpdated}");
                    }
                    else if (feedChannel is AtomFeedChannel atomFeed)
                    {
                        logger.LogDebug($"WebSub Hub: {atomFeed.WebSubHub ?? "Not available"}, Last Updated: {feedChannel.LastUpdated}");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to parse feed {FeedUri}", feedUri); return;
                }
                if (feedChannel == null) { logger.LogWarning("No feed channel parsed from feed {FeedUri}", feedUri); return; }

                if (feedChannel.Items != null && feedChannel.Items.Any())
                {
                    var cosmosConfiguration = cosmosDBOptionsMonitor.Get("FeedMonitorCosmosDBOptions");
                    if (cosmosConfiguration.Enabled)
                    {
                        // Persist the feed channel first
                        await UpsertFeedChannel2CosmosDBAsync(feedChannel, utcNow, cancellationToken);

                        // Then persist the items
                        processedCount = await UpsertFeedItems2CosmosDBAsync(feedUri, feedChannel, utcNow, cancellationToken);
                    }

                    var tableStorageConfiguration = tableStorageOptionsMonitor.Get("FeedMonitorTableStorageOptions");
                    if (tableStorageConfiguration.Enabled)
                        processedCount = await UpdateFeedItems2TableStorage(feedUri, feedChannel, utcNow, cancellationToken);

                    var blobStorageConfiguration = blobStorageOptionsMonitor.Get("FeedMonitorBlobStorageOptions");
                    if (blobStorageConfiguration.Enabled)
                    {
                        processedCount = await UpdateFeedItems2BlobStorage(feedUri, feedChannel, utcNow, cancellationToken);
                    }
                }
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing feeds");
        }
        finally
        {
            logger.LogInformation("Completed one-time plant license activation processing. Processed count: {ProcessedCount}", processedCount);
        }
    }

    private async Task UpsertFeedChannel2CosmosDBAsync(FeedChannelBase feedChannel, DateTimeOffset utcNow, CancellationToken cancellationToken)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { feedChannel, utcNow });

        try
        {
            var partitionKey = new PartitionKey(feedChannel.PartitionKey);

            // Check if channel already exists
            try
            {
                var existingResponse = await container.ReadItemObservableAsync<JObject>(
                    feedChannel.Id,
                    partitionKey,
                    cancellationToken: cancellationToken
                );

                var existingChannel = existingResponse.Resource;

                // Update LastSeen timestamp
                existingChannel["LastSeen"] = utcNow;
                existingChannel["LastUpdated"] = feedChannel.LastUpdated;
                existingChannel["DateModified"] = utcNow;

                await container.UpsertItemObservableAsync(existingChannel, partitionKey, cancellationToken: cancellationToken);
                logger.LogDebug("Updated existing feed channel {ChannelId}", feedChannel.Id);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // Channel doesn't exist on DB, create new one
                feedChannel.FirstDiscovered = utcNow;
                feedChannel.LastSeen = utcNow;
                feedChannel.DateCreated = utcNow;
                feedChannel.DateModified = utcNow;

                await container.UpsertItemAsync(feedChannel, partitionKey, cancellationToken: cancellationToken);
                logger.LogInformation($"Created new feed channel Id:{feedChannel.Id} at Uri: {feedChannel.Uri}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to upsert feed channel Id:{feedChannel.Id}");
            throw;
        }
    }

    private async Task<int> UpsertFeedItems2CosmosDBAsync(string feedUri, FeedChannelBase feedChannel, DateTimeOffset utcNow, CancellationToken cancellationToken)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { feedUri, feedChannel, utcNow });

        int processedCount = 0;
        //var utcNow = timeProvider.GetUtcNow();
        var link = (feedChannel.Link ?? new Uri(feedUri).GetLeftPart(UriPartial.Authority)).TrimEnd('/');

        var cosmosDBOptions = cosmosDBOptionsMonitor.Get("FeedMonitorCosmosDBOptions");
        var feedMonitorOptions = feedMonitorOptionsMonitor.CurrentValue;
        var loadExistingFeedsInterval = feedMonitorOptions.LoadExistingFeedsInterval;

        var minPublicationDate = TimeSpanParser.GetExpressionOccurrence(utcNow, loadExistingFeedsInterval, -1);

        bool hasNoPublicationDateItems = false;
        foreach (var item in feedChannel.Items)
        {
            item.FeedId = feedChannel.Id;
            var itemDate = item.PublicationDate;
            var year = itemDate?.Year ?? 0;
            if (year == 0) { hasNoPublicationDateItems = true; }

            item.PartitionKey = year > 0 ? $"{link}-{year}" : $"{link}";
            if (itemDate != null && minPublicationDate > itemDate.Value) { minPublicationDate = itemDate.Value; }

            // Track feed channel by ID
            if (!item.FeedChannels.Contains(feedChannel.Id))
            {
                item.FeedChannels.Add(feedChannel.Id);
            }
        }
        var feedItemsByPartition = feedChannel.Items.GroupBy(item => item.PartitionKey).ToList();

        var currentYear = utcNow.Year;
        var minYear = minPublicationDate.Year;
        var partitionKeysToQuery = new HashSet<string>();
        if (hasNoPublicationDateItems) { partitionKeysToQuery.Add($"{link}"); }
        for (int year = minYear; year <= currentYear; year++)
        {
            partitionKeysToQuery.Add($"{link}-{year}");
        }

        // Query existing items only from relevant partitions
        var allExistingItems = new Dictionary<string, List<FeedItemBase>>();

        // read items from cosmosdb current or past partition
        foreach (var partitionKeyValue in partitionKeysToQuery)
        {
            var partitionKey = new PartitionKey(partitionKeyValue);
            var existingItems = new List<FeedItemBase>();

            if (partitionKeyValue.Equals(link, StringComparison.InvariantCultureIgnoreCase))
            {
                // Get GUIDs from current feed items for this partition
                var currentItemGuids = feedChannel.Items.Where(item => item.PartitionKey == partitionKeyValue)
                                                        .Select(item => item.Guid).ToList();

                if (currentItemGuids.Any())
                {
                    var feedIterator = container.GetItemQueryIteratorObservable<FeedItemBase>(
                        queryDefinition: new QueryDefinition("SELECT * FROM c WHERE c.FeedId = @feedId AND ARRAY_CONTAINS(@guids, c.Guid)")
                            .WithParameter("@feedId", feedChannel.Id)
                            .WithParameter("@guids", currentItemGuids),
                        requestOptions: new QueryRequestOptions { PartitionKey = partitionKey }
                     );

                    while (feedIterator.HasMoreResults)
                    {
                        var response = await feedIterator.ReadNextObservableAsync(cancellationToken);
                        existingItems.AddRange(response);
                    }
                }
            }
            else
            {
                var feedIterator = container.GetItemQueryIteratorObservable<FeedItemBase>(
                  queryDefinition: new QueryDefinition("SELECT * FROM c WHERE c.FeedId = @feedId AND c.PublicationDate >= @minPublicationDate")
                    .WithParameter("@feedId", feedChannel.Id)
                    .WithParameter("@minPublicationDate", minPublicationDate),
                  requestOptions: new QueryRequestOptions { PartitionKey = partitionKey }
                );

                while (feedIterator.HasMoreResults)
                {
                    var response = await feedIterator.ReadNextObservableAsync(cancellationToken);
                    existingItems.AddRange(response);
                }
            }

            if (existingItems.Any())
            {
                allExistingItems[partitionKeyValue] = existingItems;
                logger.LogDebug("{feedUri}: existingFeedItemsListCount={ExistingCount}, partitionKey={PartitionKey}", feedUri, existingItems.Count, partitionKeyValue);
            }
        }
        logger.LogDebug("{feedUri}: Queried {PartitionCount} partition(s) for years {MinYear}-{CurrentYear}", feedUri, partitionKeysToQuery.Count, minYear, currentYear);

        foreach (var feedItemsPartitionGroup in feedItemsByPartition)
        {
            var partitionKeyValue = feedItemsPartitionGroup.Key;
            var partitionKey = new PartitionKey(partitionKeyValue);

            var existingItems = allExistingItems.GetValueOrDefault(partitionKeyValue, new List<FeedItemBase>());

            // feedItemsToUpdate => partitionGroupItem not in existingItems || existingItem.FeedChannels doesn't contain feedChannel.Id
            var feedItemsToUpsert = new List<FeedItemBase>();
            var newFeedItems = feedItemsPartitionGroup.Where(item => !existingItems.Any(existingItem => existingItem.Guid == item.Guid)).ToList();
            feedItemsToUpsert.AddRange(newFeedItems);
            var feedItemsToUpdate = feedItemsPartitionGroup.Where(item => existingItems.Any(existingItem => existingItem.Guid == item.Guid && !existingItem.FeedChannels.Contains(feedChannel.Id))).ToList();
            foreach (var item in feedItemsToUpdate)
            {
                var existingItem = existingItems.First(ei => ei.Guid == item.Guid);
                existingItem.FeedChannels.Add(feedChannel.Id);
                feedItemsToUpsert.Add(existingItem);
            }
            var existingFeedItems = feedItemsPartitionGroup.Where(item => feedItemsToUpsert.Any(existingItem => existingItem.Guid == item.Guid)).ToList();

            logger.LogInformation($"{feedUri}: feedItemsToUpsert:{feedItemsToUpsert.Count}, existingFeedItemsCount:{existingFeedItems.Count}, totalFeedItemsCount={feedItemsPartitionGroup.Count()}, existingCount={existingFeedItems.Count}, PartitionKey {partitionKeyValue}");

            // Handle new items
            if (feedItemsToUpsert != null && feedItemsToUpsert.Count > 0)
            {
                var transactionBatch = container.CreateTransactionalBatchObservable(partitionKey);
                foreach (var newItem in feedItemsToUpsert)
                {
                    transactionBatch.UpsertItem(newItem);
                    processedCount++;
                }
                var upsertResponse = await transactionBatch.ExecuteObservableAsync(cancellationToken).ConfigureAwait(false);
            }

        }

        return processedCount;
    }

    private async Task<int> UpdateFeedItems2TableStorage(string feedUri, FeedChannelBase feedChannel, DateTimeOffset urcNow, CancellationToken cancellationToken)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { feedUri, feedChannel });

        int processedCount = 0;
        var utcNow = timeProvider.GetUtcNow();
        var link = (feedChannel.Link ?? new Uri(feedUri).GetLeftPart(UriPartial.Authority)).TrimEnd('/');

        var feedMonitorOptions = feedMonitorOptionsMonitor.CurrentValue;
        var loadExistingFeedsInterval = feedMonitorOptions.LoadExistingFeedsInterval;

        var minPublicationDate = TimeSpanParser.GetExpressionOccurrence(utcNow, loadExistingFeedsInterval, -1);

        bool hasNoPublicationDateItems = false;
        foreach (var item in feedChannel.Items)
        {
            item.FeedId = feedChannel.Id;
            var itemDate = item.PublicationDate;
            var year = itemDate?.Year ?? 0;
            if (year == 0) { hasNoPublicationDateItems = true; }

            item.PartitionKey = year > 0 ? $"{link}-{year}" : $"{link}";
            if (itemDate != null && minPublicationDate > itemDate.Value) { minPublicationDate = itemDate.Value; }

            // Track feed channel by ID
            if (!item.FeedChannels.Contains(feedChannel.Id))
            {
                item.FeedChannels.Add(feedChannel.Id);
            }
        }
        var itemsByPartition = feedChannel.Items.GroupBy(item => item.PartitionKey).ToList();

        var currentYear = utcNow.Year;
        var minYear = minPublicationDate.Year;
        var partitionKeysToQuery = new HashSet<string>();
        if (hasNoPublicationDateItems) { partitionKeysToQuery.Add($"{link}"); }
        for (int year = minYear; year <= currentYear; year++)
        {
            partitionKeysToQuery.Add($"{link}-{year}");
        }

        // Query existing items from Table Storage partitions
        var allExistingItems = new Dictionary<string, HashSet<string>>();

        foreach (var partitionKeyValue in partitionKeysToQuery)
        {
            var existingItemGuids = new HashSet<string>();

            if (partitionKeyValue.Equals(link))
            {
                // Get GUIDs from current feed items for this partition
                var currentItemGuids = feedChannel.Items
                    .Where(item => item.PartitionKey == partitionKeyValue)
                 .Select(item => item.Guid)
                   .ToHashSet();

                if (currentItemGuids.Any())
                {
                    var filter = $"PartitionKey eq '{partitionKeyValue}' and FeedId eq '{feedChannel.Id}'";
                    await foreach (var entity in tableClient.QueryAsync<TableEntity>(filter, select: new[] { "RowKey" }, cancellationToken: cancellationToken))
                    {
                        var guid = entity.RowKey;
                        if (currentItemGuids.Contains(guid))
                        {
                            existingItemGuids.Add(guid);
                        }
                    }
                }
            }
            else
            {
                // Query with publication date filter
                var filter = $"PartitionKey eq '{partitionKeyValue}' and FeedId eq '{feedChannel.Id}' and PublicationDate ge datetime'{minPublicationDate:yyyy-MM-ddTHH:mm:ssZ}'";
                await foreach (var entity in tableClient.QueryAsync<TableEntity>(filter, cancellationToken: cancellationToken))
                {
                    existingItemGuids.Add(entity.RowKey);
                }
            }

            if (existingItemGuids.Any())
            {
                allExistingItems[partitionKeyValue] = existingItemGuids;
                logger.LogDebug("{feedUri}: existingItemsCount={ExistingCount}, partitionKey={PartitionKey} (Table Storage)", feedUri, existingItemGuids.Count, partitionKeyValue);
            }
        }
        logger.LogDebug("{feedUri}: Queried {PartitionCount} partition(s) for years {MinYear}-{CurrentYear} from Table Storage", feedUri, partitionKeysToQuery.Count, minYear, currentYear);

        // Upsert new items to Table Storage
        foreach (var partitionGroup in itemsByPartition)
        {
            var partitionKeyValue = partitionGroup.Key;

            var existingItemIds = allExistingItems.GetValueOrDefault(partitionKeyValue, new HashSet<string>());
            var newFeedItems = partitionGroup.Where(item => !existingItemIds.Contains(item.Guid)).ToList();
            logger.LogInformation($"{feedUri}: newFeedItemsCount:{newFeedItems.Count}, totalFeedItemsCount={partitionGroup.Count()}, existingCount={existingItemIds.Count}, PartitionKey {partitionKeyValue}");

            if (newFeedItems != null && newFeedItems.Count > 0)
            {
                foreach (var newItem in newFeedItems)
                {
                    var tableEntity = new TableEntity(partitionKeyValue, newItem.Guid)
                    {
                        { "Title", newItem.Title },
                        { "FeedId", newItem.FeedId },
                        { "Description", newItem.Description },
                        { "Link", newItem.Link },
                        { "Author", newItem.Author },
                        { "PublicationDate", newItem.PublicationDate },
                        { "LastUpdated", newItem.LastUpdated },
                        { "Categories", string.Join(";", newItem.Categories ?? []) },
                        { "FeedChannelsJson", Newtonsoft.Json.JsonConvert.SerializeObject(newItem.FeedChannels) },
                        { "DateCreated", utcNow }
                    };
                    await tableClient.UpsertEntityAsync(tableEntity, TableUpdateMode.Replace, cancellationToken);
                    processedCount++;
                }
            }
            else
            {
                logger.LogDebug("No new feed items to upsert for partition {PartitionKey}", partitionKeyValue);
            }
        }

        return processedCount;
    }

    private async Task<int> UpdateFeedItems2BlobStorage(string feedUri, FeedChannelBase feedChannel, DateTimeOffset utcNow, CancellationToken cancellationToken)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { feedUri, feedChannel, utcNow });

        int processedCount = 0;
        var link = (feedChannel.Link ?? new Uri(feedUri).GetLeftPart(UriPartial.Authority)).TrimEnd('/');
        var feedDomain = new Uri(feedUri).Host; // e.g., "devblogs.microsoft.com"

        // Sanitize and limit domain length
        var sanitizedDomain = SanitizeForBlobPath(feedDomain);
        if (sanitizedDomain.Length > 100)
        {
            sanitizedDomain = sanitizedDomain.Substring(0, 100);
            logger.LogWarning("Domain name truncated to 100 characters: {OriginalDomain} -> {SanitizedDomain}", feedDomain, sanitizedDomain);
        }

        foreach (var item in feedChannel.Items)
        {
            try
            {
                // Track feed channel by ID
                if (!item.FeedChannels.Contains(feedChannel.Id))
                {
                    item.FeedChannels.Add(feedChannel.Id);
                }

                // Determine publication date (use current date if not available)
                var pubDate = item.PublicationDate ?? utcNow.DateTime;
                var year = pubDate.Year;
                var month = pubDate.Month;

                // Create a sanitized, unique folder name
                var sanitizedTitle = SanitizeForBlobPath(item.Title ?? item.Guid);
                var dateTimePrefix = pubDate.ToString("yyyyMMdd HHmmss");

                // Limit title to 40 chars to leave room for other components
                var titlePart = sanitizedTitle.Length > 43 ? sanitizedTitle.Substring(0, 40) + "..." : sanitizedTitle;

                // Extract and limit the unique identifier from the GUID
                var guidPart = ExtractUniqueIdentifier(item.Guid, feedDomain);

                // Structure: feeds/{domain}/{year}/{month}/{yyyyMMdd HHmmss} {title} ({guid-id})/
                var blobPrefix = $"feeds/{sanitizedDomain}/{year:D4}/{month:D2}/{dateTimePrefix} {titlePart} ({guidPart})";

                // Validate total path length (leave buffer under 1024 limit)
                const int maxPathLength = 1000;
                if (blobPrefix.Length > maxPathLength)
                {
                    // Fallback to minimal path using hash
                    var hashCode = item.Guid.GetHashCode().ToString("X8");
                    blobPrefix = $"feeds/{sanitizedDomain}/{year:D4}/{month:D2}/{dateTimePrefix}-{hashCode}";
                    logger.LogWarning("Blob path too long, using shortened version for item {ItemGuid}: {BlobPrefix}", item.Guid, blobPrefix);
                }

                // Check if item already exists
                var metadataPath = $"{blobPrefix}/metadata.json";
                var metadataBlob = blobContainerClient.GetBlobClient(metadataPath);

                if (await metadataBlob.ExistsAsync(cancellationToken))
                {
                    logger.LogDebug("Blob already exists: {BlobPath}", metadataPath);
                    continue;
                }

                // Determine content source based on feed type
                string contentToUpload = null;
                if (item is RSSFeedItem rssItem && !string.IsNullOrEmpty(rssItem.ContentEncoded))
                {
                    // RSS feeds use content:encoded element
                    contentToUpload = rssItem.ContentEncoded;
                }
                else if (item is AtomFeedItem atomItem && atomItem.Content != null && !string.IsNullOrEmpty(atomItem.Content.Text))
                {
                    // Atom feeds use content element
                    contentToUpload = atomItem.Content.Text;
                }

                // Upload content.md
                if (!string.IsNullOrEmpty(contentToUpload))
                {
                    var contentPath = $"{blobPrefix}/content.md";
                    await UploadBlobAsync(contentPath, contentToUpload, "text/markdown", cancellationToken);
                }

                // Upload description.md
                if (!string.IsNullOrEmpty(item.Description))
                {
                    var descriptionPath = $"{blobPrefix}/description.md";
                    await UploadBlobAsync(descriptionPath, item.Description, "text/markdown", cancellationToken);
                }

                // Upload metadata.json with full item details
                var metadata = new
                {
                    item.Guid,
                    item.Title,
                    item.Link,
                    item.Author,
                    item.PublicationDate,
                    item.LastUpdated,
                    item.Categories,
                    FeedId = item.FeedId,
                    FeedUri = feedUri,
                    FeedTitle = feedChannel.Title,
                    FeedChannels = item.FeedChannels,
                    ProcessedDate = utcNow
                };

                var metadataJson = Newtonsoft.Json.JsonConvert.SerializeObject(metadata, Newtonsoft.Json.Formatting.Indented);
                await UploadBlobAsync(metadataPath, metadataJson, "application/json", cancellationToken);

                processedCount++;
                logger.LogDebug("Uploaded feed item to blob: {BlobPrefix}", blobPrefix);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to upload feed item {ItemGuid} to blob storage", item.Guid);
            }
        }

        logger.LogInformation("{FeedUri}: Uploaded {ProcessedCount} new items to blob storage", feedUri, processedCount);
        return processedCount;
    }

    private async Task UploadBlobAsync(string blobPath, string content, string contentType, CancellationToken cancellationToken)
    {
        var blobClient = blobContainerClient.GetBlobClient(blobPath);
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

        var uploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        await blobClient.UploadAsync(stream, uploadOptions, cancellationToken);
    }

    /// <summary>
    /// Extracts a unique identifier from the GUID, removing the domain if present
    /// </summary>
    /// <param name="guid">The item GUID (e.g., "https://devblogs.microsoft.com/?p=20083")</param>
    /// <param name="feedDomain">The feed domain to remove (e.g., "devblogs.microsoft.com")</param>
    /// <returns>Sanitized unique identifier (e.g., "p=20083")</returns>
    private static string ExtractUniqueIdentifier(string guid, string feedDomain)
    {
        if (string.IsNullOrEmpty(guid)) return "unknown";

        try
        {
            // Try to parse as URI
            if (Uri.TryCreate(guid, UriKind.Absolute, out var uri))
            {
                // Extract query string or path segments
                var uniquePart = !string.IsNullOrEmpty(uri.Query)
      ? uri.Query.TrimStart('?')  // e.g., "p=20083"
: uri.AbsolutePath.TrimStart('/');  // e.g., "2024/01/post-name"

                // If still contains the domain, try to remove it
                uniquePart = uniquePart.Replace(feedDomain, "").Trim('/', '-');

                var sanitized = SanitizeForBlobPath(uniquePart);

                // Limit length to 50 characters to ensure total path stays within limits
                return sanitized.Length > 50 ? sanitized.Substring(0, 50) : sanitized;
            }

            // If not a valid URI, just sanitize the GUID as-is
            var result = SanitizeForBlobPath(guid);
            return result.Length > 50 ? result.Substring(0, 50) : result;
        }
        catch
        {
            var fallback = SanitizeForBlobPath(guid);
            return fallback.Length > 50 ? fallback.Substring(0, 50) : fallback;
        }
    }



    private static string SanitizeForBlobPath(string input)
    {
        var invalid = new char[] { '<', '>', ':', '"', '\\', '|', '?', '*', '\0' };
        // This removes '/' which is actually VALID and REQUIRED for blob hierarchies
        var sanitized = new string(input.Select(c => invalid.Contains(c) ? '-' : c).ToArray());
        sanitized = Regex.Replace(sanitized, "-+", "-").Trim('-', ' ');
        return string.IsNullOrEmpty(sanitized) ? "untitled" : sanitized;
    }

    private static CosmosClientOptions GetCosmosClientOptions(CosmosDBClientConfiguration cosmosDBClientConfiguration)
    {
        var logger = Observability.LoggerFactory.CreateLogger<FeedMonitorBackgroundService>();
        var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { cosmosDBClientConfiguration });

        var cosmosClientOptions = new CosmosClientOptions
        {
            ConnectionMode = cosmosDBClientConfiguration.ConnectionMode,
            RequestTimeout = cosmosDBClientConfiguration.RequestTimeout,
            MaxRequestsPerTcpConnection = cosmosDBClientConfiguration.MaxRequestsPerTcpConnection,
            MaxRetryAttemptsOnRateLimitedRequests = cosmosDBClientConfiguration.MaxRetryAttemptsOnThrottledRequests,
            MaxRetryWaitTimeOnRateLimitedRequests = TimeSpan.FromSeconds(cosmosDBClientConfiguration.MaxRetryWaitTimeInSeconds),
            Serializer = NewtonsoftJsonCosmosSerializer.Instance,
            CosmosClientTelemetryOptions = new CosmosClientTelemetryOptions() { DisableDistributedTracing = false }
        };

        return cosmosClientOptions;
    }

    private static BlobClientOptions GetBlobClientOptions(BlobClientConfiguration blobClientConfiguration)
    {
        var logger = Observability.LoggerFactory.CreateLogger<FeedMonitorBackgroundService>();
        var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { blobClientConfiguration });

        // Note: CustomerProvidedKey and TransferValidation are not configurable via BlobClientOptions
        // They must be set per-operation on BlobClient methods
        BlobClientOptions blobClientOptions;
        if (!string.IsNullOrEmpty(blobClientConfiguration.ServiceVersion))
        {
            if (Enum.TryParse<BlobClientOptions.ServiceVersion>(blobClientConfiguration.ServiceVersion, out var blobServiceVersion))
            {
                blobClientOptions = new BlobClientOptions(blobServiceVersion);
            }
            else
            {
                blobClientOptions = new BlobClientOptions();
            }
        }
        else
        {
            blobClientOptions = new BlobClientOptions();
        }

        // Audience
        if (!string.IsNullOrEmpty(blobClientConfiguration.Audience))
        {
            if (Enum.TryParse<BlobAudience>(blobClientConfiguration.Audience, out var audience))
            {
                blobClientOptions.Audience = audience;
            }
            else
            {
                // Try as custom audience URI
                blobClientOptions.Audience = new BlobAudience(blobClientConfiguration.Audience);
            }
        }

        // Enable Tenant Discovery
        blobClientOptions.EnableTenantDiscovery = blobClientConfiguration.EnableTenantDiscovery;

        // Encryption Scope
        if (!string.IsNullOrEmpty(blobClientConfiguration.EncryptionScope))
        {
            blobClientOptions.EncryptionScope = blobClientConfiguration.EncryptionScope;
        }

        // Geo-Redundant Secondary URI
        if (!string.IsNullOrEmpty(blobClientConfiguration.GeoRedundantSecondaryUri))
        {
            blobClientOptions.GeoRedundantSecondaryUri = new Uri(blobClientConfiguration.GeoRedundantSecondaryUri);
        }

        // Trim Blob Name Slashes
        blobClientOptions.TrimBlobNameSlashes = blobClientConfiguration.TrimBlobNameSlashes;

        // Retry Configuration
        if (blobClientConfiguration.Retry != null)
        {
            var retryConfig = blobClientConfiguration.Retry;

            if (retryConfig.MaxRetries.HasValue)
            {
                blobClientOptions.Retry.MaxRetries = retryConfig.MaxRetries.Value;
            }

            if (!string.IsNullOrEmpty(retryConfig.Mode))
            {
                if (Enum.TryParse<Azure.Core.RetryMode>(retryConfig.Mode, out var retryMode))
                {
                    blobClientOptions.Retry.Mode = retryMode;
                }
            }

            if (retryConfig.DelaySeconds.HasValue)
            {
                blobClientOptions.Retry.Delay = TimeSpan.FromSeconds(retryConfig.DelaySeconds.Value);
            }

            if (retryConfig.MaxDelaySeconds.HasValue)
            {
                blobClientOptions.Retry.MaxDelay = TimeSpan.FromSeconds(retryConfig.MaxDelaySeconds.Value);
            }

            if (retryConfig.NetworkTimeoutSeconds.HasValue)
            {
                blobClientOptions.Retry.NetworkTimeout = TimeSpan.FromSeconds(retryConfig.NetworkTimeoutSeconds.Value);
            }
        }

        // Diagnostics Configuration
        if (blobClientConfiguration.Diagnostics != null)
        {
            var diagConfig = blobClientConfiguration.Diagnostics;

            blobClientOptions.Diagnostics.IsLoggingEnabled = diagConfig.IsLoggingEnabled;
            blobClientOptions.Diagnostics.IsDistributedTracingEnabled = diagConfig.IsDistributedTracingEnabled;
            blobClientOptions.Diagnostics.IsLoggingContentEnabled = diagConfig.IsLoggingContentEnabled;

            if (diagConfig.IsTelemetryEnabled.HasValue)
            {
                blobClientOptions.Diagnostics.IsTelemetryEnabled = diagConfig.IsTelemetryEnabled.Value;
            }

            if (!string.IsNullOrEmpty(diagConfig.ApplicationId))
            {
                blobClientOptions.Diagnostics.ApplicationId = diagConfig.ApplicationId;
            }
        }

        return blobClientOptions;
    }

    private static TableClientOptions GetTableClientOptions(TableClientConfiguration tableClientConfiguration)
    {
        var logger = Observability.LoggerFactory.CreateLogger<FeedMonitorBackgroundService>();
        var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { tableClientConfiguration });

        TableClientOptions tableClientOptions;

        if (!string.IsNullOrEmpty(tableClientConfiguration.ServiceVersion))
        {
            if (Enum.TryParse<TableClientOptions.ServiceVersion>(tableClientConfiguration.ServiceVersion, out var tableServiceVersion))
            {
                tableClientOptions = new TableClientOptions(tableServiceVersion);
            }
            else
            {
                tableClientOptions = new TableClientOptions();
            }
        }
        else
        {
            tableClientOptions = new TableClientOptions();
        }

        // Audience
        if (!string.IsNullOrEmpty(tableClientConfiguration.Audience))
        {
            if (Enum.TryParse<TableAudience>(tableClientConfiguration.Audience, out var audience))
            {
                tableClientOptions.Audience = audience;
            }
            else
            {
                // Try as custom audience URI
                tableClientOptions.Audience = new TableAudience(tableClientConfiguration.Audience);
            }
        }

        // Enable Tenant Discovery
        tableClientOptions.EnableTenantDiscovery = tableClientConfiguration.EnableTenantDiscovery;

        // Retry Configuration
        if (tableClientConfiguration.Retry != null)
        {
            var retryConfig = tableClientConfiguration.Retry;

            if (retryConfig.MaxRetries.HasValue)
            {
                tableClientOptions.Retry.MaxRetries = retryConfig.MaxRetries.Value;
            }

            if (!string.IsNullOrEmpty(retryConfig.Mode))
            {
                if (Enum.TryParse<Azure.Core.RetryMode>(retryConfig.Mode, out var retryMode))
                {
                    tableClientOptions.Retry.Mode = retryMode;
                }
            }

            if (retryConfig.DelaySeconds.HasValue)
            {
                tableClientOptions.Retry.Delay = TimeSpan.FromSeconds(retryConfig.DelaySeconds.Value);
            }

            if (retryConfig.MaxDelaySeconds.HasValue)
            {
                tableClientOptions.Retry.MaxDelay = TimeSpan.FromSeconds(retryConfig.MaxDelaySeconds.Value);
            }

            if (retryConfig.NetworkTimeoutSeconds.HasValue)
            {
                tableClientOptions.Retry.NetworkTimeout = TimeSpan.FromSeconds(retryConfig.NetworkTimeoutSeconds.Value);
            }
        }

        // Diagnostics Configuration
        if (tableClientConfiguration.Diagnostics != null)
        {
            var diagConfig = tableClientConfiguration.Diagnostics;

            tableClientOptions.Diagnostics.IsLoggingEnabled = diagConfig.IsLoggingEnabled;
            tableClientOptions.Diagnostics.IsDistributedTracingEnabled = diagConfig.IsDistributedTracingEnabled;
            tableClientOptions.Diagnostics.IsLoggingContentEnabled = diagConfig.IsLoggingContentEnabled;

            if (diagConfig.IsTelemetryEnabled.HasValue)
            {
                tableClientOptions.Diagnostics.IsTelemetryEnabled = diagConfig.IsTelemetryEnabled.Value;
            }

            if (!string.IsNullOrEmpty(diagConfig.ApplicationId))
            {
                tableClientOptions.Diagnostics.ApplicationId = diagConfig.ApplicationId;
            }
        }

        return tableClientOptions;
    }

    private CosmosClient GetCosmosClient(CosmosDBClientConfiguration cosmosDBClientConfiguration)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { cosmosDBClientConfiguration });

        CosmosClient cosmosClient;
        CosmosClientOptions cosmosClientOptions = GetCosmosClientOptions(cosmosDBClientConfiguration);

        // Create CosmosClient using appropriate authentication method
        if (!string.IsNullOrEmpty(cosmosDBClientConfiguration.ConnectionString))
        {
            // Method 1: Connection String
            cosmosClient = new CosmosClient(cosmosDBClientConfiguration.ConnectionString, cosmosClientOptions);
        }
        else if (cosmosDBClientConfiguration.EndpointUri != null)
        {
            // Method 2: Endpoint URI with AuthKey
            if (!string.IsNullOrEmpty(cosmosDBClientConfiguration.AuthKey))
            {
                cosmosClient = new CosmosClient(cosmosDBClientConfiguration.EndpointUri.ToString(), cosmosDBClientConfiguration.AuthKey, cosmosClientOptions);
            }
            else
            {
                // Method 3: Azure AD / Managed Identity (DefaultAzureCredential)
                var credentialProvider = new DefaultCredentialProvider(environment);
                var credential = credentialProvider.Get(configuration.GetSection("FeedMonitor"));

                cosmosClient = new CosmosClient(cosmosDBClientConfiguration.EndpointUri.ToString(), credential, cosmosClientOptions);
            }
        }
        else
        {
            throw new InvalidOperationException("CosmosDB configuration requires either ConnectionString or EndpointUri");
        }

        return cosmosClient;
    }

    private BlobServiceClient GetBlobServiceClient(BlobClientConfiguration blobClientConfiguration)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { blobClientConfiguration });
        BlobServiceClient blobServiceClient;
        BlobClientOptions blobClientOptions = GetBlobClientOptions(blobClientConfiguration);

        // Create BlobServiceClient using appropriate authentication method
        if (!string.IsNullOrEmpty(blobClientConfiguration.ConnectionString))
        {
            // Method 1: Connection String
            blobServiceClient = new BlobServiceClient(blobClientConfiguration.ConnectionString, blobClientOptions);
        }
        else if (!string.IsNullOrEmpty(blobClientConfiguration.EndpointUri))
        {
            var endpoint = new Uri(blobClientConfiguration.EndpointUri);

            if (!string.IsNullOrEmpty(blobClientConfiguration.SasToken))
            {
                // Method 2: SAS Token
                blobServiceClient = new BlobServiceClient(endpoint, new Azure.AzureSasCredential(blobClientConfiguration.SasToken), blobClientOptions);
            }
            else if (!string.IsNullOrEmpty(blobClientConfiguration.AccountName) && !string.IsNullOrEmpty(blobClientConfiguration.AccountKey))
            {
                // Method 3: Shared Key
                blobServiceClient = new BlobServiceClient(endpoint, new Azure.Storage.StorageSharedKeyCredential(blobClientConfiguration.AccountName, blobClientConfiguration.AccountKey), blobClientOptions);
            }
            else
            {
                // Method 4: Azure AD / Managed Identity (DefaultAzureCredential)
                blobServiceClient = new BlobServiceClient(endpoint, new DefaultAzureCredential(), blobClientOptions);
            }
        }
        else
        {
            throw new InvalidOperationException("Blob Storage configuration requires either ConnectionString or EndpointUri");
        }
        return blobServiceClient;
    }

    private TableServiceClient GetTableServiceClient(TableClientConfiguration tableClientConfiguration)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { tableClientConfiguration });

        TableServiceClient tableServiceClient;

        TableClientOptions tableClientOptions = GetTableClientOptions(tableClientConfiguration);

        // Create TableServiceClient using appropriate authentication method
        if (!string.IsNullOrEmpty(tableClientConfiguration.ConnectionString))
        {
            // Method 1: Connection String
            tableServiceClient = new TableServiceClient(tableClientConfiguration.ConnectionString, tableClientOptions);
        }
        else if (!string.IsNullOrEmpty(tableClientConfiguration.EndpointUri))
        {
            var endpoint = new Uri(tableClientConfiguration.EndpointUri);

            if (!string.IsNullOrEmpty(tableClientConfiguration.SasToken))
            {
                // Method 2: SAS Token
                tableServiceClient = new TableServiceClient(endpoint, new Azure.AzureSasCredential(tableClientConfiguration.SasToken), tableClientOptions);
            }
            else if (!string.IsNullOrEmpty(tableClientConfiguration.AccountName) && !string.IsNullOrEmpty(tableClientConfiguration.AccountKey))
            {
                // Method 3: Shared Key
                tableServiceClient = new TableServiceClient(endpoint, new Azure.Data.Tables.TableSharedKeyCredential(tableClientConfiguration.AccountName, tableClientConfiguration.AccountKey), tableClientOptions);
            }
            else
            {
                // Method 4: Azure AD / Managed Identity (DefaultAzureCredential)
                tableServiceClient = new TableServiceClient(endpoint, new DefaultAzureCredential(), tableClientOptions);
            }
        }
        else
        {
            throw new InvalidOperationException("Table Storage configuration requires either ConnectionString or EndpointUri");
        }
        return tableServiceClient;
    }
}
