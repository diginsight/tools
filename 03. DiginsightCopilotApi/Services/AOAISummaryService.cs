using Azure.AI.OpenAI;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.ResourceGraph;
using Azure.ResourceManager.ResourceGraph.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Diginsight;
using Diginsight.Diagnostics;
using DiginsightCopilotApi.Abstractions;
using DiginsightCopilotApi.Configuration;
using DiginsightCopilotApi.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Collections;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using TokenCredential = Azure.Core.TokenCredential;

namespace DiginsightCopilotApi.Services;

public class AOAISummaryService : ISummaryService
{
    private static readonly string resourceGraphEndpoint = "https://management.azure.com/providers/Microsoft.ResourceGraph/resources?api-version=2022-10-01";
    private readonly ILogger<AOAISummaryService> logger;

    private IOptions<AzureResourcesOptions> azureResourcesOptions;
    private IOptions<AzureDevopsOptions> devopsOptions;
    private IOptions<HttpContextOptions> httpOptions;
    private IOptions<AzureOpenAiOptions> openAiOptions;
    private IOptions<BlobStorageOptions> blobStorageOptions;
    private IOptions<PromptOptions> promptOptions;
    private IOptions<AzureAdOptions> azureAdOptions;


    private OpenAIClient openAiClient;
    private AzureOpenAIClient azureOpenAiClient;


    public AOAISummaryService(
        ILogger<AOAISummaryService> logger,
        IOptions<AzureOpenAiOptions> openAiOptions,
        IOptions<AzureDevopsOptions> devopsOptions,
        IOptions<HttpContextOptions> httpOptions,
        IOptions<BlobStorageOptions> blobStorageOptions,
        IOptions<PromptOptions> promptOptions,
        IOptions<AzureAdOptions> azureAdOptions,
        IOptions<AzureResourcesOptions> azureResourcesOptions,
        IOptions<PromptOptions> promptConfig
        )
    {
        this.logger = logger;
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { openAiOptions, devopsOptions });

        this.azureAdOptions = azureAdOptions;
        this.httpOptions = httpOptions;
        this.openAiOptions = openAiOptions;
        this.blobStorageOptions = blobStorageOptions;
        this.promptOptions = promptOptions;
        this.devopsOptions = devopsOptions;
        this.azureResourcesOptions = azureResourcesOptions;

        var openAiConfig = openAiOptions.Value;
        this.azureOpenAiClient = new AzureOpenAIClient(new Uri(openAiConfig.Endpoint), new ApiKeyCredential(openAiOptions.Value.ApiKey));

    }
    private static async Task<string> GetAccessTokenAsync(TokenCredential credential)
    {
        var tokenRequestContext = new TokenRequestContext(new[] { "https://management.azure.com/.default" });
        AccessToken token = await credential.GetTokenAsync(tokenRequestContext, cancellationToken: default);
        return token.Token;
    }
    private async Task<string> GetAccessTokenAsync()
    {
        var tenantId = azureAdOptions.Value.TenantId;
        var clientId = azureAdOptions.Value.ClientId;
        var clientSecret = azureAdOptions.Value.ClientSecret;

        using (var httpClient = new HttpClient())
        {
            var url = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("scope", "https://management.azure.com/.default"),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            dynamic json = JsonConvert.DeserializeObject(responseBody);
            return json.access_token;
        }
    }
    private async Task GetApplicationInsightResourceAsync(string instrumentationKey, string accessToken)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { instrumentationKey });

        var subscriptionId = azureResourcesOptions.Value.SubscriptionId;

        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var query = new
            {
                subscriptions = new[] { subscriptionId },
                query = $""" 
                         Resources
                         | where type == 'microsoft.insights/components'
                         | where properties.InstrumentationKey == '{instrumentationKey}'
                         | project id
                         | take 1
                         """
            };
            logger.LogDebug("query: {query}", query);

            var content = new StringContent(JsonConvert.SerializeObject(query), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(resourceGraphEndpoint, content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
        }

    }

    private async Task<ResourceQueryResult> GetApplicationInsightResourceAsync(string instrumentationKey, TokenCredential tokenCredential)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { instrumentationKey });

        var armClient = new ArmClient(tokenCredential);
        var tenantCollection = armClient.GetTenants();
        var tenants = tenantCollection.GetAllAsync(cancellationToken: default);
        var tenant = await tenants.FirstAsync(cancellationToken: default);
        var subscriptions = tenant.GetSubscriptions();

        var query = $""" 
                     Resources
                     | where type == 'microsoft.insights/components'
                     | where properties.InstrumentationKey == '{instrumentationKey}'
                     | project id
                     | take 1
                     """;

        var subscriptionId = azureResourcesOptions.Value.SubscriptionId;
        var queryContent = new ResourceQueryContent(query);
        queryContent.Subscriptions.Add(subscriptionId);

        var response = await tenant.GetResourcesAsync(queryContent);
        return response.Value; // ResourceGroup, Name, SubscriptionId
    }

    public async Task<Analysis> GenerateSummary(string logContent, int buildId, IEnumerable<WorkItemParam> workItems, IEnumerable<ChangeParam> changes, IEnumerable<AssemblyMetadata> assemblyMetadata)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logContent, buildId, workItems, changes });

        var openAiConfig = openAiOptions.Value;
        var client = azureOpenAiClient.GetChatClient(openAiConfig.ChatModel); logger.LogDebug($"var client = azureOpenAiClient.GetChatClient({openAiConfig.ChatModel});");

        var nowOffset = DateTimeOffset.Now;
        var nowOffsetUtc = DateTimeOffset.UtcNow;

        List<PromptChatMessage>? messages = null;
        var promptConfig = promptOptions.Value;
        var promptFolder = promptConfig.PromptFolder;
        var promptFileName = string.IsNullOrEmpty(promptFolder) ? "01.LogSummarize.prompt.yaml" : $"{promptFolder}\\01.LogSummarize.prompt.yaml";

        var promptYamlTemplate = File.ReadAllText(promptFileName);
        var deserializer = new DeserializerBuilder()
                           .WithNamingConvention(new PascalCaseNamingConvention())
                           .Build();
        var yamlObject = deserializer.Deserialize(new StringReader(promptYamlTemplate)) as IList<object>;

        var blobStorageConfig = this.blobStorageOptions.Value;
        var blobServiceClient = new BlobServiceClient(blobStorageConfig.BlobStorageConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient("analysis");
        var analysisSasToken = containerClient.GenerateSasUri(BlobContainerSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1)).Query;
        logger.LogDebug("analysisSasToken: {analysisSasToken}", analysisSasToken);

        var instrumentationKey = azureResourcesOptions.Value.InstrumentationKey;
        if (!string.IsNullOrEmpty(instrumentationKey))
        {
            var tenantId = azureAdOptions.Value.TenantId;
            var clientId = azureAdOptions.Value.ClientId;
            var clientSecret = azureAdOptions.Value.ClientSecret;
            var tokenCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            var resourceQueryResult = await GetApplicationInsightResourceAsync(instrumentationKey, tokenCredential);
            var jsonDocument = JsonDocument.Parse(resourceQueryResult.Data.ToString());
            var root = jsonDocument?.RootElement != null && jsonDocument.RootElement.GetArrayLength() > 0 ? jsonDocument.RootElement[0] : default(JsonElement);
            var applicationInsightId = root.GetProperty("id").ToString();

            azureResourcesOptions.Value.ApplicationInsightId = applicationInsightId;
            if (!string.IsNullOrEmpty(applicationInsightId))
            {
                var applicationInsightIdStringPattern = "/subscriptions/(.*)/resourceGroups/(.*)/providers/microsoft.insights/components/(.*)";
                var applicationInsightIdStringMatch = Regex.Match(applicationInsightId, applicationInsightIdStringPattern);
                if (applicationInsightIdStringMatch.Success)
                {
                    var subscriptionId = applicationInsightIdStringMatch.Groups[1].Value;
                    var resourceGroup = applicationInsightIdStringMatch.Groups[2].Value;
                    var applicationInsightName = applicationInsightIdStringMatch.Groups[3].Value;
                    logger.LogDebug("subscriptionId: {subscriptionId}, resourceGroup: {resourceGroup}, applicationInsightName: {applicationInsightName}, applicationId: {applicationId}", subscriptionId, resourceGroup, applicationInsightName);

                    azureResourcesOptions.Value.SubscriptionId = subscriptionId;
                    azureResourcesOptions.Value.ApplicationInsightName = applicationInsightName;
                    azureResourcesOptions.Value.ApplicationInsightResourceGroup = resourceGroup;
                }
            }
        }

        string folderNamePrefix = $"{DateTime.UtcNow:yyyyMMdd HHmm} - ";
        string logFileName = $"{folderNamePrefix}LogStream";
        var azureResourcesConfig = this.azureResourcesOptions.Value;
        var devopsConfig = this.azureResourcesOptions.Value;
        var httpConfig = this.httpOptions.Value;
        var traceId = "666d0447c75de5e945abd60b949a8e2f";

        // DBUG 666d0447c75de5e945abd60b949a8e2f              1  LandingCallMiddleware.InvokeAsync
        //"DBUG (\w+)(.*)LandingCallMiddleware.InvokeAsync"


        // Title FileName SASToken FileName
        List<ChatMessage> chatMessages = new();
        foreach (var messageObject in yamlObject)
        {
            var message = messageObject as IDictionary<object, object>;
            var requestHeaders = httpConfig.Headers;
            message["Value"] = PromptReplacePlaceholders(message["Value"] as string, new { nowOffsetUtc, logContent, devopsConfig, httpConfig, azureResourcesConfig, changes, buildId, analysisSasToken, assemblyMetadata, requestHeaders, workItems, folderNamePrefix, logFileName, traceId });
            if (message["Type"].Equals("SystemChatMessage")) { chatMessages.Add(new SystemChatMessage(message["Value"] as string)); }
            else if (message["Type"].Equals("UserChatMessage")) { chatMessages.Add(new UserChatMessage(message["Value"] as string)); }
        }

        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"; logger.LogDebug("isDevelopment: {isDevelopment}", isDevelopment);
        if (isDevelopment)
        {
            var serializer = new SerializerBuilder()
                                .WithNamingConvention(new CamelCaseNamingConvention())
                                .Build();
            var actualPrompt = serializer.Serialize(yamlObject);
            var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var applicationName = AppDomain.CurrentDomain.FriendlyName;
            var actualPromptFolder = $"{userProfilePath}\\{applicationName}";
            Directory.CreateDirectory(actualPromptFolder);
            var actualPromptPath = $"{actualPromptFolder}\\DevopsSummarize.prompt.actual.yaml";
            logger.LogDebug("actualPromptPath: {actualPromptPath}", actualPromptPath);
            await File.WriteAllTextAsync(actualPromptPath, actualPrompt); logger.LogDebug($"await File.WriteAllTextAsync({actualPromptPath}, actualPrompt);");
        }

        logger.LogDebug($"before client.CompleteChatAsync({chatMessages});");
        var response = await client.CompleteChatAsync(chatMessages);
        logger.LogDebug($"{response} = await client.CompleteChatAsync({chatMessages});");
        var ret = response.Value.Content?.FirstOrDefault()?.Text ?? "";

        var doc = new HtmlDocument();
        doc.LoadHtml(ret);
        var titleNode = doc.DocumentNode.SelectSingleNode("//title");
        var title = titleNode.InnerText.Trim();

        string folderName = $"{folderNamePrefix}{title}";

        string analysisFileName = $"{folderName}";
        var analysisBlobClient = containerClient.GetBlobClient($"{folderName}/{analysisFileName}.htm");
        using var analysisStream = new MemoryStream(Encoding.UTF8.GetBytes(ret));
        await analysisBlobClient.UploadAsync(analysisStream, overwrite: true);

        var logBlobClientLog = containerClient.GetBlobClient($"{folderName}/{logFileName}.log");
        using var logStream = new MemoryStream(Encoding.UTF8.GetBytes(logContent));
        await logBlobClientLog.UploadAsync(logStream, overwrite: true);


        var analysisFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{analysisFileName}.htm{analysisSasToken}";
        var logFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{logFileName}.log{analysisSasToken}";

        var analysis = new Analysis()
        {
            Title = title,
            Description = "",
            Details = ret,
            Url = analysisFileUrl,
            LogUrl = logFileUrl,
        };

        activity?.SetOutput(analysis);
        return analysis;
    }

    private string PromptReplacePlaceholders(string message, object variables)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { message, variables });
        if (variables == null) { return message; }

        var variablesDic = new Dictionary<string, object?>();
        foreach (var prop in variables.GetType().GetProperties())
        {
            variablesDic.Add(prop.Name, prop.GetValue(variables));
        }
        message = PromptReplacePlaceholders(message, variablesDic);

        activity?.SetOutput(message);
        return message;
    }
    private string PromptReplacePlaceholders(string message, IDictionary<string, object?> variables)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { message, variables });
        if (variables == null) { return message; }

        foreach (var propName in variables.Keys)
        {
            var value = variables[propName];
            if (value == null) { continue; }
            var type = value.GetType();
            if (type.IsPrimitive || type == typeof(decimal) || type == typeof(string) || type == typeof(DateTime) || type == typeof(DateTimeOffset))
            {
                message = message.Replace($"{{{{{propName}}}}}", value.ToString());
            }
            else if (IsEnumerableButNotPrimitive(value))
            {
                var valueProperties = value.GetType().GetProperties().Where(p => p.GetIndexParameters().Length == 0);
                foreach (var itemProp in valueProperties)
                {
                    object? itemValue = null;
                    try { itemValue = itemProp.GetValue(value); } catch { continue; }
                    message = message.Replace($"{{{{{propName}.{itemProp.Name}}}}}", itemValue?.ToString());
                }

                var start = "{{#each {{propName}}}}".Replace("{{propName}}", propName);
                var end = "{{/each}}";

                var pattern = $"{start}(.*?){end}";
                var match = Regex.Match(message, pattern, RegexOptions.Singleline);
                var itemHtmlTemplate = match.Groups[0].Value;
                if (match.Groups == null || match.Groups.Count == 0 || string.IsNullOrEmpty(itemHtmlTemplate)) { continue; }

                itemHtmlTemplate = itemHtmlTemplate.Replace(start, "");
                itemHtmlTemplate = itemHtmlTemplate.Replace(end, "");
                var itemsText = new List<string>();
                foreach (var item in (value as IEnumerable)!)
                {
                    var dic = new Dictionary<string, object?>(variables);
                    dic.Remove(propName);
                    dic.Add(propName.TrimEnd('s'), item);
                    var workItemHtml = PromptReplacePlaceholders(itemHtmlTemplate, dic);
                    itemsText.Add(workItemHtml.Trim());
                }
                message = Regex.Replace(message, pattern, string.Join("\r\n", itemsText), RegexOptions.Singleline);
            }
            else
            {
                var itemProperties = value.GetType().GetProperties().Where(p => p.GetIndexParameters().Length == 0);
                foreach (var itemProp in itemProperties)
                {
                    object? itemValue = null;
                    try { itemValue = itemProp.GetValue(value); } catch { continue; }

                    message = message.Replace($"{{{{{propName}.{itemProp.Name}}}}}", itemValue?.ToString());
                }
            }
        }

        activity?.SetOutput(message);
        return message;
    }
    public static bool IsEnumerableButNotPrimitive(object value)
    {
        if (value is IEnumerable && !(value is string))
        {
            System.Type type = value.GetType();
            return !type.IsPrimitive && type != typeof(decimal);
        }
        return false;
    }


    public class AccessTokenCredential : TokenCredential
    {
        private readonly string _token;

        public AccessTokenCredential(string token)
        {
            _token = token;
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, System.Threading.CancellationToken cancellationToken)
        {
            return new AccessToken(_token, DateTimeOffset.MaxValue);
        }

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, System.Threading.CancellationToken cancellationToken)
        {
            return new ValueTask<AccessToken>(new AccessToken(_token, DateTimeOffset.MaxValue));
        }
    }
}
