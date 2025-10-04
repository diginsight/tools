using Cocona;
using Diginsight.Diagnostics;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace CosmosdbConsole;

internal sealed class Executor : IDisposable
{
    private readonly ILogger logger;
    private readonly CosmosClient cosmosClient;
    private readonly Container container;
    private readonly string? file;
    private readonly bool whatIf;
    private readonly int? top;
    private readonly string? transformString = """

        """;

    private string? transform(string? recordJson) { 
        if (recordJson is null) { return null; }

        var document = JObject.Parse(recordJson);
        var latitudeProp = document.Properties().Where(p => p.Name.StartsWith("Latitude")).FirstOrDefault();
        var longitudeProp = document.Properties().Where(p => p.Name.StartsWith("Longitude")).FirstOrDefault();
        var latitude = latitudeProp?.Value is not null ? (int)Double.Parse(latitudeProp.Value.ToString()): 0;
        var longitude = longitudeProp?.Value is not null ? (int)Double.Parse(longitudeProp.Value.ToString()): 0;
        var coodKey = $"{latitude},{longitude}";

        var partitionKeyProp = document.Properties().Where(p => p.Name.StartsWith("partitionKey")).FirstOrDefault();
        if (partitionKeyProp == null)
        {
            document.Add("partitionKey", coodKey);
        }
        return document.ToString();
    }

    public Executor(ILogger<Executor> logger)
    {
        this.logger = logger;

        using Activity? activity = Observability.ActivitySource.StartMethodActivity(logger);

    }

    public void Dispose()
    {
        cosmosClient?.Dispose();
    }

    public async Task QueryAsync(
        [FromService] CoconaAppContext appContext,
        [Option('c')] string connectionString,
        [Option('q')] string query,
        [Option('d')] string database,
        [Option('t')] string collection,
        [Option('f')] string? file,
        [Option("x")] string? transformFile,
        [Option] int top = -1,
        [Option] int skip = 0
    )
    {
        using Activity? activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { query, file, top, skip });

        try
        {
            string accountEndpoint = connectionString.Split(';').Select(static x => x.Split('=', 2)).First(static x => x[0].Equals("AccountEndpoint", StringComparison.OrdinalIgnoreCase))[1];
            logger.LogDebug("accountEndpoint: {accountEndpoint}", accountEndpoint);

            var cosmosClient = new CosmosClient(connectionString); logger.LogDebug("cosmosClient = new CosmosClient(connectionString);");
            var container = cosmosClient.GetContainer(database, collection); logger.LogDebug($"container = cosmosClient.GetContainer({database}, {collection});");

            var topClause = top > 0 ? $" OFFSET {skip} LIMIT {top}" : string.Empty;
            string modifiedQuery = $"{query}{topClause}";
            logger.LogDebug("modifiedQuery: {modifiedQuery}", modifiedQuery);

            var requestOptions = new QueryRequestOptions { MaxItemCount = top, QueryTextMode = QueryTextMode.None };
            var iterator = container.GetItemQueryStreamIterator(modifiedQuery, requestOptions: requestOptions);

            StreamWriter? streamWriter = null;
            if (file is not null)
            {
                streamWriter = new StreamWriter(file); logger.LogInformation($"streamWriter = new StreamWriter({file});");
            }

            using (streamWriter)
            {
                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    if (!response.IsSuccessStatusCode) { throw new Exception(response.ErrorMessage); }

                    response.Content.Position = 0;
                    var content = await new System.IO.StreamReader(response.Content).ReadToEndAsync();
                    logger.LogDebug("content: {content}", content);
                    if (streamWriter is not null)
                    {
                        await streamWriter.WriteAsync(content);
                    }
                }
            }

        }
        catch (Exception ex) { logger.LogError(ex, $"'{ex.GetType().Name}': {ex.Message}", ex); }
    }

    public async Task StreamDocumentsJsonAsync(
        [FromService] CoconaAppContext appContext,
        [Option('f')] string? filePath,
        [Option('x')] string? transformFile,
        [Option('s')] string? skipFields,
        [Option] int top = -1,
        [Option] int skip = 0
    )
    {
        using Activity? activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { filePath });

        try
        {
            using (var streamReader = new StreamReader(filePath))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                logger.LogDebug($"Read until the 'Documents' property is found");
                while (await jsonReader.ReadAsync())
                {
                    if (jsonReader.TokenType == JsonToken.PropertyName && (string)jsonReader.Value == "Documents")
                    {
                        await jsonReader.ReadAsync(); // Move to the start of the array
                        if (jsonReader.TokenType == JsonToken.StartArray)
                        {
                            logger.LogDebug($"Start of the array is found");
                            break;
                        }
                    }
                }

                int documentCount = 0;
                var skipFieldsArray = skipFields?.Split(',')?.Select(static x => x.Trim())?.ToArray();
                logger.LogDebug($"Read documents within the 'Documents' array");
                while (await jsonReader.ReadAsync() && jsonReader.TokenType != JsonToken.EndArray)
                {
                    if (jsonReader.TokenType == JsonToken.StartObject)
                    {
                        var document = await JObject.LoadAsync(jsonReader);
                        NormalizeDocument(document, skipFieldsArray);

                        //var documentString = document.ToString();
                        //documentString = transform(documentString);


                        documentCount++;
                    }
                }
            }
        }
        catch (Exception ex) { logger.LogError(ex, $"'{ex.GetType().Name}': {ex.Message}", ex); }

    }

    public async Task UploadDocumentsJsonAsync(
        [FromService] CoconaAppContext appContext,
        [Option('f')] string filePath,
        [Option('c')] string connectionString,
        [Option('d')] string database,
        [Option('t')] string collection,
        [Option('s')] string skipFields,
        [Option] int top = -1,
        [Option] int skip = 0
    )
    {
        using Activity? activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { filePath, database, collection, skipFields, top, skip });

        try
        {
            string accountEndpoint = connectionString.Split(';').Select(static x => x.Split('=', 2)).First(static x => x[0].Equals("AccountEndpoint", StringComparison.OrdinalIgnoreCase))[1];
            logger.LogDebug("accountEndpoint: {accountEndpoint}", accountEndpoint);

            var cosmosClient = new CosmosClient(connectionString); logger.LogDebug("cosmosClient = new CosmosClient(connectionString);");
            var container = cosmosClient.GetContainer(database, collection); logger.LogDebug($"container = cosmosClient.GetContainer({database}, {collection});");

            using (var streamReader = new StreamReader(filePath))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                logger.LogDebug($"Read until the 'Documents' property is found");
                while (await jsonReader.ReadAsync())
                {
                    if (jsonReader.TokenType == JsonToken.PropertyName && (string)jsonReader.Value == "Documents")
                    {
                        await jsonReader.ReadAsync(); // Move to the start of the array
                        if (jsonReader.TokenType == JsonToken.StartArray)
                        {
                            logger.LogDebug($"Start of the array is found");
                            break;
                        }
                    }
                }

                int documentCount = 0;
                var skipFieldsArray = skipFields?.Split(',')?.Select(static x => x.Trim())?.ToArray();
                logger.LogDebug($"Read documents within the 'Documents' array");
                while (await jsonReader.ReadAsync() && jsonReader.TokenType != JsonToken.EndArray)
                {
                    if (jsonReader.TokenType == JsonToken.StartObject)
                    {
                        var document = await JObject.LoadAsync(jsonReader);
                        var id = NormalizeDocument(document, skipFieldsArray);

                        var response = await container.UpsertItemAsync(document); 
                        id = GetDocumentId(document); logger.LogDebug($"container.UpsertItemAsync(document {{{id}}});");

                        documentCount++;
                    }
                }
            }

        }
        catch (Exception ex) { logger.LogError(ex, $"'{ex.GetType().Name}': {ex.Message}", ex); }
    }

    public async Task DeleteDocumentsFromJsonAsync(
       [FromService] CoconaAppContext appContext,
       [Option('f')] string filePath,
       [Option('c')] string connectionString,
       [Option('d')] string database,
       [Option('t')] string collection,
       [Option] int top = -1,
       [Option] int skip = 0
   )
    {
        using Activity? activity = Observability.ActivitySource.StartMethodActivity(logger, () => new { filePath, database, collection, top, skip });

        try
        {
            string accountEndpoint = connectionString.Split(';').Select(static x => x.Split('=', 2)).First(static x => x[0].Equals("AccountEndpoint", StringComparison.OrdinalIgnoreCase))[1];
            logger.LogDebug("accountEndpoint: {accountEndpoint}", accountEndpoint);

            var cosmosClient = new CosmosClient(connectionString); logger.LogDebug("cosmosClient = new CosmosClient(connectionString);");
            var container = cosmosClient.GetContainer(database, collection); logger.LogDebug($"container = cosmosClient.GetContainer({database}, {collection});");

            using (var streamReader = new StreamReader(filePath))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                logger.LogDebug($"Read until the 'Documents' property is found");
                while (await jsonReader.ReadAsync())
                {
                    if (jsonReader.TokenType == JsonToken.PropertyName && (string)jsonReader.Value == "Documents")
                    {
                        await jsonReader.ReadAsync(); // Move to the start of the array
                        if (jsonReader.TokenType == JsonToken.StartArray)
                        {
                            logger.LogDebug($"Start of the array is found");
                            break;
                        }
                    }
                }

                int documentCount = 0;
                logger.LogDebug($"Read documents within the 'Documents' array");
                while (await jsonReader.ReadAsync() && jsonReader.TokenType != JsonToken.EndArray)
                {
                    if (jsonReader.TokenType == JsonToken.StartObject)
                    {
                        var document = await JObject.LoadAsync(jsonReader);
                        var id = GetDocumentId(document);

                        await container.DeleteItemStreamAsync(id, PartitionKey.None); logger.LogDebug($"container.DeleteItemStreamAsync({id}, PartitionKey.None);");
                        //await container.ReadItemStreamAsync(id); logger.LogDebug($"container.UpsertItemAsync(document);");
                        documentCount++;
                    }
                }
            }

        }
        catch (Exception ex) { logger.LogError(ex, $"'{ex.GetType().Name}': {ex.Message}", ex); }
    }

    private string NormalizeDocument(JObject document, string[] skipFields)
    {
        var skipProperties = document.Properties().Where(p => p.Name.StartsWith("_") || skipFields is not null && skipFields.Contains(p.Name, StringComparer.InvariantCultureIgnoreCase))?.ToList();
        foreach (var property in skipProperties)
        {
            property.Remove();
        }
        var idProp = document.Properties().Where(p => p.Name.StartsWith("id")).FirstOrDefault();
        if (idProp == null) {
            var id = Guid.NewGuid();
            document.Add("id", id.ToString());
        }
        return idProp?.Value?.ToString();
    }
    private string GetDocumentId(JObject document)
    {
        var idProp = document.Properties().Where(p => p.Name.StartsWith("id")).FirstOrDefault();
        return idProp.Value.ToString();
    }

}
