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
using Microsoft.TeamFoundation.Test.WebApi;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Collections;
using System.Drawing.Imaging;
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

    public async Task<Analysis> GenerateFullAnalysis(string logContent, string title, IDictionary<string, object> placeholders)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logContent, title, placeholders });

        var openAiConfig = openAiOptions.Value;
        var client = azureOpenAiClient.GetChatClient(openAiConfig.ChatModel); logger.LogDebug($"var client = azureOpenAiClient.GetChatClient({openAiConfig.ChatModel});");

        var timeInformation = placeholders.TryGetValue("timeInformation", out var timeInformationObj) ? (TimeInformation)timeInformationObj : new TimeInformation();
        var nowOffsetUtc = timeInformation.UtcNow;

        List<PromptChatMessage>? messages = null;
        var promptConfig = promptOptions.Value;
        var promptFolder = promptConfig.PromptFolder;
        var promptName = "01.00 - GenerateFullAnalysis";
        var promptFileName = string.IsNullOrEmpty(promptFolder) ? $"{promptName}.prompt.yaml" : $"{promptFolder}\\{promptName}.prompt.yaml";

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

        string folderNamePrefix = $"{nowOffsetUtc.DateTime:yyyyMMdd HHmm} - ";
        string logFileName = $"{folderNamePrefix}LogStream";
        var azureResourcesInformation = this.azureResourcesOptions.Value;
        var devopsInformation = this.devopsOptions.Value;
        var httpInformation = this.httpOptions.Value;
        var azureAdInformation = this.azureAdOptions.Value;

        var otherInformation = placeholders.TryGetValue("otherInformation", out var otherInformationObj) ? (Dictionary<string, object?>)otherInformationObj : new Dictionary<string, object?>();
        if (!otherInformation.ContainsKey("analysisSasToken")) { otherInformation.Add("analysisSasToken", analysisSasToken); }
        if (!otherInformation.ContainsKey("folderNamePrefix")) { otherInformation.Add("folderNamePrefix", folderNamePrefix); }
        if (!otherInformation.ContainsKey("logFileName")) { otherInformation.Add("logFileName", logFileName); }

        List<ChatMessage> chatMessages = new();
        foreach (var messageObject in yamlObject)
        {
            var message = messageObject as IDictionary<object, object>;
            var requestHeaders = httpInformation.Headers;
            message["Value"] = PromptReplacePlaceholders(message["Value"] as string, placeholders);

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
        //var title = titleNode.InnerText.Trim();

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

        activity?.SetOutput(analysis?.ToString()?.GetLogString());
        return analysis;
    }

    public async Task<Analysis> GenerateTitle(string logContent, IDictionary<string, object> placeholders)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logContent, placeholders });

        var openAiConfig = openAiOptions.Value;
        var client = azureOpenAiClient.GetChatClient(openAiConfig.ChatModel); logger.LogDebug($"var client = azureOpenAiClient.GetChatClient({openAiConfig.ChatModel});");

        var timeInformation = placeholders.TryGetValue("timeInformation", out var timeInformationObj) ? (TimeInformation)timeInformationObj : new TimeInformation();
        var nowOffsetUtc = timeInformation.UtcNow;

        List<PromptChatMessage>? messages = null;
        var promptConfig = promptOptions.Value;
        var promptFolder = promptConfig.TemplateFolder;
        var promptName = "01.01 - GenerateTitle";
        var promptFileName = string.IsNullOrEmpty(promptFolder) ? $"{promptName}.prompt.yaml" : $"{promptFolder}\\{promptName}.prompt.yaml";

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

        string folderNamePrefix = $"{nowOffsetUtc.DateTime:yyyyMMdd HHmm} - ";
        string logFileName = $"{folderNamePrefix}LogStream";
        var azureResourcesInformation = this.azureResourcesOptions.Value;
        var devopsInformation = this.devopsOptions.Value;
        var httpInformation = this.httpOptions.Value;
        var azureAdInformation = this.azureAdOptions.Value;

        var otherInformation = placeholders.TryGetValue("otherInformation", out var otherInformationObj) ? (Dictionary<string, object?>)otherInformationObj : new Dictionary<string, object?>();
        if (!otherInformation.ContainsKey("analysisSasToken")) { otherInformation.Add("analysisSasToken", analysisSasToken); }
        if (!otherInformation.ContainsKey("folderNamePrefix")) { otherInformation.Add("folderNamePrefix", folderNamePrefix); }
        if (!otherInformation.ContainsKey("logFileName")) { otherInformation.Add("logFileName", logFileName); }

        List<ChatMessage> chatMessages = new();
        foreach (var messageObject in yamlObject)
        {
            var message = messageObject as IDictionary<object, object>;
            var requestHeaders = httpInformation.Headers;
            message["Value"] = PromptReplacePlaceholders(message["Value"] as string, placeholders);

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
            var actualPromptPath = $"{actualPromptFolder}\\{promptName}.prompt.actual.yaml";
            logger.LogDebug("actualPromptPath: {actualPromptPath}", actualPromptPath);
            await File.WriteAllTextAsync(actualPromptPath, actualPrompt); logger.LogDebug($"await File.WriteAllTextAsync({actualPromptPath}, actualPrompt);");
        }

        logger.LogDebug($"before client.CompleteChatAsync({chatMessages});");
        var response = await client.CompleteChatAsync(chatMessages);
        logger.LogDebug($"{response} = await client.CompleteChatAsync({chatMessages});");
        var ret = response.Value.Content?.FirstOrDefault()?.Text ?? "";

        var markdownContentPattern = "```html(.*)```";
        var match = Regex.Match(ret, markdownContentPattern, RegexOptions.Singleline);
        var itemHtmlResponse = match.Groups[1].Value?.Trim();
        if (string.IsNullOrEmpty(itemHtmlResponse)) { itemHtmlResponse = ret; }

        var doc = new HtmlDocument();
        doc.LoadHtml(itemHtmlResponse);
        var titleNode = doc.DocumentNode.SelectSingleNode("/section/h1");
        var title = titleNode.InnerText.Trim();

        var inferredInformation = placeholders.TryGetValue("inferredInformation", out var inferredInformationObj) ? (IDictionary<string, object?>)inferredInformationObj : new Dictionary<string, object?>();
        if (!inferredInformation.ContainsKey("Title")) { inferredInformation.Add("Title", title); }

        string folderName = $"{folderNamePrefix}{title}";

        string analysisFileName = $"{promptName}";
        var analysisBlobClient = containerClient.GetBlobClient($"{folderName}/{analysisFileName}.htm");
        using var analysisStream = new MemoryStream(Encoding.UTF8.GetBytes(itemHtmlResponse));
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
            Details = itemHtmlResponse,
            Url = analysisFileUrl,
            LogUrl = logFileUrl,
        };

        activity?.SetOutput(analysis);
        return analysis;
    }
    public async Task<Analysis> InferPlaceholders(string logContent, string title, IDictionary<string, object> placeholders)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logContent, title, placeholders });

        var openAiConfig = openAiOptions.Value;
        var client = azureOpenAiClient.GetChatClient(openAiConfig.ChatModel); logger.LogDebug($"var client = azureOpenAiClient.GetChatClient({openAiConfig.ChatModel});");

        var timeInformation = placeholders.TryGetValue("timeInformation", out var timeInformationObj) ? (TimeInformation)timeInformationObj : new TimeInformation();
        var nowOffsetUtc = timeInformation.UtcNow;

        List<PromptChatMessage>? messages = null;
        var promptConfig = promptOptions.Value;
        var promptFolder = promptConfig.TemplateFolder;
        var promptName = "01.02 - InferPlaceholders";
        var promptFileName = string.IsNullOrEmpty(promptFolder) ? $"{promptName}.prompt.yaml" : $"{promptFolder}\\{promptName}.prompt.yaml";

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

        string folderNamePrefix = $"{nowOffsetUtc.DateTime:yyyyMMdd HHmm} - ";
        string logFileName = $"{folderNamePrefix}LogStream";
        var azureResourcesInformation = this.azureResourcesOptions.Value;
        var devopsInformation = this.devopsOptions.Value;
        var httpInformation = this.httpOptions.Value;
        var azureAdInformation = this.azureAdOptions.Value;

        var otherInformation = placeholders.TryGetValue("otherInformation", out var otherInformationObj) ? (Dictionary<string, object?>)otherInformationObj : new Dictionary<string, object?>();
        if (!otherInformation.ContainsKey("analysisSasToken")) { otherInformation.Add("analysisSasToken", analysisSasToken); }
        if (!otherInformation.ContainsKey("folderNamePrefix")) { otherInformation.Add("folderNamePrefix", folderNamePrefix); }
        if (!otherInformation.ContainsKey("logFileName")) { otherInformation.Add("logFileName", logFileName); }

        List<ChatMessage> chatMessages = new();
        foreach (var messageObject in yamlObject)
        {
            var message = messageObject as IDictionary<object, object>;
            var requestHeaders = httpInformation.Headers;
            message["Value"] = PromptReplacePlaceholders(message["Value"] as string, placeholders);

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
            var actualPromptPath = $"{actualPromptFolder}\\{promptName}.prompt.actual.yaml";
            logger.LogDebug("actualPromptPath: {actualPromptPath}", actualPromptPath);
            await File.WriteAllTextAsync(actualPromptPath, actualPrompt); logger.LogDebug($"await File.WriteAllTextAsync({actualPromptPath}, actualPrompt);");
        }

        logger.LogDebug($"before client.CompleteChatAsync({chatMessages});");
        var response = await client.CompleteChatAsync(chatMessages);
        logger.LogDebug($"{response} = await client.CompleteChatAsync({chatMessages});");
        var ret = response.Value.Content?.FirstOrDefault()?.Text ?? "";

        var markdownContentPattern = "```html(.*)```";
        var match = Regex.Match(ret, markdownContentPattern, RegexOptions.Singleline);
        var itemHtmlResponse = match.Groups[1].Value?.Trim();
        if (string.IsNullOrEmpty(itemHtmlResponse)) { itemHtmlResponse = ret; }

        var inferredPlaceholders = GetInferredPlaceholders(itemHtmlResponse);

        var inferredInformation = placeholders.TryGetValue("inferredInformation", out var inferredInformationObj) ? (IDictionary<string, object?>)inferredInformationObj : new Dictionary<string, object?>();
        var userInformation = placeholders.TryGetValue("userInformation", out var userInformationObj) ? (UserInformation)userInformationObj : new UserInformation();
        foreach (var placeholder in inferredPlaceholders)
        {
            if (!inferredInformation.ContainsKey(placeholder.Key)) { inferredInformation.Add(placeholder.Key.Trim(':'), placeholder.Value); }
            if (placeholder.Key == "userDisplayName:") { userInformation.DisplayName = placeholder.Value?.ToString()!; }
            if (placeholder.Key == "userEmail:") { userInformation.DisplayName = placeholder.Value?.ToString()!; }
        }

        string folderName = $"{folderNamePrefix}{title}";

        string analysisFileName = $"{promptName}";
        var analysisBlobClient = containerClient.GetBlobClient($"{folderName}/{analysisFileName}.htm");
        using var analysisStream = new MemoryStream(Encoding.UTF8.GetBytes(itemHtmlResponse));
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
            Details = itemHtmlResponse,
            Url = analysisFileUrl,
            LogUrl = logFileUrl,
        };

        activity?.SetOutput(analysis);
        return analysis;
    }
    public async Task<Analysis> GenerateApplicationFlowInformation(string logContent, string title, IDictionary<string, object> placeholders)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logContent, title, placeholders });

        var timeInformation = placeholders.TryGetValue("timeInformation", out var timeInformationObj) ? (TimeInformation)timeInformationObj : new TimeInformation();
        var nowOffsetUtc = timeInformation.UtcNow;

        var promptConfig = promptOptions.Value;
        var templateFolder = promptConfig.TemplateFolder;
        var templateName = "02.02 - ApplicationFlowInformation";
        var templateFileName = string.IsNullOrEmpty(templateFolder) ? $"{templateName}Template.html" : $"{templateFolder}\\{templateName}Template.html";
        var htmlTemplateContent = File.ReadAllText(templateFileName);

        var blobStorageConfig = this.blobStorageOptions.Value;
        var blobServiceClient = new BlobServiceClient(blobStorageConfig.BlobStorageConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient("analysis");
        var analysisSasToken = containerClient.GenerateSasUri(BlobContainerSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1)).Query;
        logger.LogDebug("analysisSasToken: {analysisSasToken}", analysisSasToken);

        string folderNamePrefix = $"{nowOffsetUtc.DateTime:yyyyMMdd HHmm} - ";
        string logFileName = $"{folderNamePrefix}LogStream";

        var otherInformation = placeholders.TryGetValue("otherInformation", out var otherInformationObj) ? (Dictionary<string, object?>)otherInformationObj : new Dictionary<string, object?>();
        if (!otherInformation.ContainsKey("analysisSasToken")) { otherInformation.Add("analysisSasToken", analysisSasToken); }
        if (!otherInformation.ContainsKey("folderNamePrefix")) { otherInformation.Add("folderNamePrefix", folderNamePrefix); }
        if (!otherInformation.ContainsKey("logFileName")) { otherInformation.Add("logFileName", logFileName); }

        var htmlItemResponse = PromptReplacePlaceholders(htmlTemplateContent, placeholders);

        //var doc = new HtmlDocument();
        //doc.LoadHtml(itemHtmlResponse);
        //var titleNode = doc.DocumentNode.SelectSingleNode("/section/h1");
        //var title = titleNode.InnerText.Trim();

        string folderName = $"{folderNamePrefix}{title}";

        string analysisFileName = $"{templateName}";
        var analysisBlobClient = containerClient.GetBlobClient($"{folderName}/{analysisFileName}.htm");
        using var analysisStream = new MemoryStream(Encoding.UTF8.GetBytes(htmlItemResponse));
        await analysisBlobClient.UploadAsync(analysisStream, overwrite: true);
        if (!otherInformation.ContainsKey(templateName)) { otherInformation.Add(templateName, htmlItemResponse); }

        var logBlobClientLog = containerClient.GetBlobClient($"{folderName}/{logFileName}.log");
        using var logStream = new MemoryStream(Encoding.UTF8.GetBytes(logContent));
        await logBlobClientLog.UploadAsync(logStream, overwrite: true);

        var analysisFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{analysisFileName}.htm{analysisSasToken}";
        var logFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{logFileName}.log{analysisSasToken}";

        var analysis = new Analysis()
        {
            Title = title,
            Description = "",
            Details = htmlItemResponse,
            Url = analysisFileUrl,
            LogUrl = logFileUrl,
        };

        activity?.SetOutput(analysis);
        return analysis;
    }
    public async Task<Analysis> GenerateSummary(string logContent, string title, IDictionary<string, object> placeholders)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logContent, title, placeholders });

        var openAiConfig = openAiOptions.Value;
        var client = azureOpenAiClient.GetChatClient(openAiConfig.ChatModel); logger.LogDebug($"var client = azureOpenAiClient.GetChatClient({openAiConfig.ChatModel});");

        var timeInformation = placeholders.TryGetValue("timeInformation", out var timeInformationObj) ? (TimeInformation)timeInformationObj : new TimeInformation();
        var nowOffsetUtc = timeInformation.UtcNow;

        List<PromptChatMessage>? messages = null;
        var promptConfig = promptOptions.Value;
        var templateFolder = promptConfig.TemplateFolder;
        var templateName = "02.03 - SummaryInformation";
        var promptFileName = string.IsNullOrEmpty(templateFolder) ? $"{templateName}.prompt.yaml" : $"{templateFolder}\\{templateName}.prompt.yaml";
        var templateFileName = string.IsNullOrEmpty(templateFolder) ? $"{templateName}.template.html" : $"{templateFolder}\\{templateName}.template.html";
        string analysisFileName = $"{templateName}";

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

        string folderNamePrefix = $"{nowOffsetUtc.DateTime:yyyyMMdd HHmm} - ";
        string folderName = $"{folderNamePrefix}{title}";
        string logFileName = $"{folderNamePrefix}LogStream";
        var azureResourcesInformation = this.azureResourcesOptions.Value;
        var devopsInformation = this.devopsOptions.Value;
        var httpInformation = this.httpOptions.Value;
        var azureAdInformation = this.azureAdOptions.Value;

        var otherInformation = placeholders.TryGetValue("otherInformation", out var otherInformationObj) ? (Dictionary<string, object?>)otherInformationObj : new Dictionary<string, object?>();
        if (!otherInformation.ContainsKey("analysisSasToken")) { otherInformation.Add("analysisSasToken", analysisSasToken); }
        if (!otherInformation.ContainsKey("folderNamePrefix")) { otherInformation.Add("folderNamePrefix", folderNamePrefix); }
        if (!otherInformation.ContainsKey("logFileName")) { otherInformation.Add("logFileName", logFileName); }

        List<ChatMessage> chatMessages = new();
        foreach (var messageObject in yamlObject)
        {
            var message = messageObject as IDictionary<object, object>;
            var requestHeaders = httpInformation.Headers;
            message["Value"] = PromptReplacePlaceholders(message["Value"] as string, placeholders);

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
            var actualPromptPath = $"{actualPromptFolder}\\{templateName}.prompt.actual.yaml";
            logger.LogDebug("actualPromptPath: {actualPromptPath}", actualPromptPath);
            await File.WriteAllTextAsync(actualPromptPath, actualPrompt); logger.LogDebug($"await File.WriteAllTextAsync({actualPromptPath}, actualPrompt);");
        }

        logger.LogDebug($"before client.CompleteChatAsync({chatMessages});");
        var response = await client.CompleteChatAsync(chatMessages);
        logger.LogDebug($"{response} = await client.CompleteChatAsync({chatMessages});");
        var ret = response.Value.Content?.FirstOrDefault()?.Text ?? "";

        var markdownContentPattern = "```html(.*)```";
        var match = Regex.Match(ret, markdownContentPattern, RegexOptions.Singleline);
        var promptItemResponse = match.Groups[1].Value?.Trim();
        if (string.IsNullOrEmpty(promptItemResponse)) { promptItemResponse = ret; }

        string promptAnalysisFileName = $"{templateName}";
        var promptAnalysisBlobClient = containerClient.GetBlobClient($"{folderName}/{promptAnalysisFileName}.prompt.htm");
        using var promptAnalysisStream = new MemoryStream(Encoding.UTF8.GetBytes(promptItemResponse));
        await promptAnalysisBlobClient.UploadAsync(promptAnalysisStream, overwrite: true);

        var doc = new HtmlDocument();
        doc.LoadHtml(promptItemResponse);
        var titleNode = doc.DocumentNode.SelectSingleNode("/section");
        var summaryAnalysisContent = titleNode.InnerHtml.Trim();
        summaryAnalysisContent = summaryAnalysisContent.Replace("<h3>Summary</h3>", "").Trim();
        if (!otherInformation.ContainsKey("summaryAnalysisContent")) { otherInformation.Add("summaryAnalysisContent", summaryAnalysisContent); }

        var htmlTemplateFileName = string.IsNullOrEmpty(templateFolder) ? $"{templateName}Template.html" : $"{templateFolder}\\{templateName}Template.html";
        var htmlTemplateContent = File.ReadAllText(templateFileName);
        var htmlItemResponse = PromptReplacePlaceholders(htmlTemplateContent, placeholders);

        var analysisBlobClient = containerClient.GetBlobClient($"{folderName}/{analysisFileName}.htm");
        using var analysisStream1 = new MemoryStream(Encoding.UTF8.GetBytes(htmlItemResponse));
        await analysisBlobClient.UploadAsync(analysisStream1, overwrite: true);
        
        if (!otherInformation.ContainsKey(templateName)) { otherInformation.Add(templateName, htmlItemResponse); }

        var analysisFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{analysisFileName}.htm{analysisSasToken}";
        var logFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{logFileName}.log{analysisSasToken}";

        var analysis = new Analysis()
        {
            Title = title,
            Description = "",
            Details = promptItemResponse,
            Url = analysisFileUrl,
            LogUrl = logFileUrl,
        };

        activity?.SetOutput(analysis);
        return analysis;
    }
    public async Task<Analysis> GenerateResources(string logContent, string title, IDictionary<string, object> placeholders)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logContent, title, placeholders });

        var openAiConfig = openAiOptions.Value;
        var client = azureOpenAiClient.GetChatClient(openAiConfig.ChatModel); logger.LogDebug($"var client = azureOpenAiClient.GetChatClient({openAiConfig.ChatModel});");

        var timeInformation = placeholders.TryGetValue("timeInformation", out var timeInformationObj) ? (TimeInformation)timeInformationObj : new TimeInformation();
        var nowOffsetUtc = timeInformation.UtcNow;

        List<PromptChatMessage>? messages = null;
        var promptConfig = promptOptions.Value;
        var templateFolder = promptConfig.TemplateFolder;
        var templateName = "02.04 - Resources";
        var promptFileName = string.IsNullOrEmpty(templateFolder) ? $"{templateName}.prompt.yaml" : $"{templateFolder}\\{templateName}.prompt.yaml";
        var templateFileName = string.IsNullOrEmpty(templateFolder) ? $"{templateName}.template.html" : $"{templateFolder}\\{templateName}.template.html";
        string analysisFileName = $"{templateName}";

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

        string folderNamePrefix = $"{nowOffsetUtc.DateTime:yyyyMMdd HHmm} - ";
        string folderName = $"{folderNamePrefix}{title}";
        string logFileName = $"{folderNamePrefix}LogStream";
        var azureResourcesInformation = this.azureResourcesOptions.Value;
        var devopsInformation = this.devopsOptions.Value;
        var httpInformation = this.httpOptions.Value;
        var azureAdInformation = this.azureAdOptions.Value;

        var otherInformation = placeholders.TryGetValue("otherInformation", out var otherInformationObj) ? (Dictionary<string, object?>)otherInformationObj : new Dictionary<string, object?>();
        if (!otherInformation.ContainsKey("analysisSasToken")) { otherInformation.Add("analysisSasToken", analysisSasToken); }
        if (!otherInformation.ContainsKey("folderNamePrefix")) { otherInformation.Add("folderNamePrefix", folderNamePrefix); }
        if (!otherInformation.ContainsKey("logFileName")) { otherInformation.Add("logFileName", logFileName); }

        List<ChatMessage> chatMessages = new();
        foreach (var messageObject in yamlObject)
        {
            var message = messageObject as IDictionary<object, object>;
            var requestHeaders = httpInformation.Headers;
            message["Value"] = PromptReplacePlaceholders(message["Value"] as string, placeholders);

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
            var actualPromptPath = $"{actualPromptFolder}\\{templateName}.prompt.actual.yaml";
            logger.LogDebug("actualPromptPath: {actualPromptPath}", actualPromptPath);
            await File.WriteAllTextAsync(actualPromptPath, actualPrompt); logger.LogDebug($"await File.WriteAllTextAsync({actualPromptPath}, actualPrompt);");
        }

        logger.LogDebug($"before client.CompleteChatAsync({chatMessages});");
        var response = await client.CompleteChatAsync(chatMessages);
        logger.LogDebug($"{response} = await client.CompleteChatAsync({chatMessages});");
        var ret = response.Value.Content?.FirstOrDefault()?.Text ?? "";

        var markdownContentPattern = "```html(.*)```";
        var match = Regex.Match(ret, markdownContentPattern, RegexOptions.Singleline);
        var promptItemResponse = match.Groups[1].Value?.Trim();
        if (string.IsNullOrEmpty(promptItemResponse)) { promptItemResponse = ret; }

        string promptAnalysisFileName = $"{templateName}";
        var promptAnalysisBlobClient = containerClient.GetBlobClient($"{folderName}/{promptAnalysisFileName}.prompt.htm");
        using var promptAnalysisStream = new MemoryStream(Encoding.UTF8.GetBytes(promptItemResponse));
        await promptAnalysisBlobClient.UploadAsync(promptAnalysisStream, overwrite: true);

        var doc = new HtmlDocument();
        doc.LoadHtml(promptItemResponse);
        var titleNode = doc.DocumentNode.SelectSingleNode("/section");
        var resourcesAnalysisContent = titleNode.InnerHtml.Trim();
        resourcesAnalysisContent = resourcesAnalysisContent.Replace("<h3>Application Flow Resources</h3>", "").Trim();
        if (!otherInformation.ContainsKey("resourcesAnalysisContent")) { otherInformation.Add("resourcesAnalysisContent", resourcesAnalysisContent); }

        var htmlTemplateFileName = string.IsNullOrEmpty(templateFolder) ? $"{templateName}Template.html" : $"{templateFolder}\\{templateName}Template.html";
        var htmlTemplateContent = File.ReadAllText(templateFileName);
        var htmlItemResponse = PromptReplacePlaceholders(htmlTemplateContent, placeholders);

        var analysisBlobClient = containerClient.GetBlobClient($"{folderName}/{analysisFileName}.htm");
        using var analysisStream1 = new MemoryStream(Encoding.UTF8.GetBytes(htmlItemResponse));
        await analysisBlobClient.UploadAsync(analysisStream1, overwrite: true);

        if (!otherInformation.ContainsKey(templateName)) { otherInformation.Add(templateName, htmlItemResponse); }

        var analysisFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{analysisFileName}.htm{analysisSasToken}";
        var logFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{logFileName}.log{analysisSasToken}";

        var analysis = new Analysis()
        {
            Title = title,
            Description = "",
            Details = promptItemResponse,
            Url = analysisFileUrl,
            LogUrl = logFileUrl,
        };

        activity?.SetOutput(analysis);
        return analysis;
    }
    public async Task<Analysis> GenerateReference(string logContent, string title, IDictionary<string, object> placeholders)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logContent, title, placeholders });

        var openAiConfig = openAiOptions.Value;
        var client = azureOpenAiClient.GetChatClient(openAiConfig.ChatModel); logger.LogDebug($"var client = azureOpenAiClient.GetChatClient({openAiConfig.ChatModel});");

        var timeInformation = placeholders.TryGetValue("timeInformation", out var timeInformationObj) ? (TimeInformation)timeInformationObj : new TimeInformation();
        var nowOffsetUtc = timeInformation.UtcNow;

        List<PromptChatMessage>? messages = null;
        var promptConfig = promptOptions.Value;
        var templateFolder = promptConfig.TemplateFolder;
        var templateName = "02.05 - Reference";
        var templateFileName = string.IsNullOrEmpty(templateFolder) ? $"{templateName}.template.html" : $"{templateFolder}\\{templateName}.template.html";
        string analysisFileName = $"{templateName}";

        var blobStorageConfig = this.blobStorageOptions.Value;
        var blobServiceClient = new BlobServiceClient(blobStorageConfig.BlobStorageConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient("analysis");
        var analysisSasToken = containerClient.GenerateSasUri(BlobContainerSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1)).Query;
        logger.LogDebug("analysisSasToken: {analysisSasToken}", analysisSasToken);

        string folderNamePrefix = $"{nowOffsetUtc.DateTime:yyyyMMdd HHmm} - ";
        string folderName = $"{folderNamePrefix}{title}";
        string logFileName = $"{folderNamePrefix}LogStream";
        var azureResourcesInformation = this.azureResourcesOptions.Value;
        var devopsInformation = this.devopsOptions.Value;
        var httpInformation = this.httpOptions.Value;
        var azureAdInformation = this.azureAdOptions.Value;
        var otherInformation = placeholders.TryGetValue("otherInformation", out var otherInformationObj) ? (Dictionary<string, object?>)otherInformationObj : new Dictionary<string, object?>();
        if (!otherInformation.ContainsKey("analysisSasToken")) { otherInformation.Add("analysisSasToken", analysisSasToken); }
        if (!otherInformation.ContainsKey("folderNamePrefix")) { otherInformation.Add("folderNamePrefix", folderNamePrefix); }
        if (!otherInformation.ContainsKey("logFileName")) { otherInformation.Add("logFileName", logFileName); }

        var htmlTemplateFileName = string.IsNullOrEmpty(templateFolder) ? $"{templateName}Template.html" : $"{templateFolder}\\{templateName}Template.html";
        var htmlTemplateContent = File.ReadAllText(templateFileName);
        var htmlItemResponse = PromptReplacePlaceholders(htmlTemplateContent, placeholders);

        var analysisBlobClient = containerClient.GetBlobClient($"{folderName}/{analysisFileName}.htm");
        using var analysisStream = new MemoryStream(Encoding.UTF8.GetBytes(htmlItemResponse));
        await analysisBlobClient.UploadAsync(analysisStream, overwrite: true);

        if (!otherInformation.ContainsKey(templateName)) { otherInformation.Add(templateName, htmlItemResponse); }

        var analysisFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{analysisFileName}.htm{analysisSasToken}";
        var logFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{logFileName}.log{analysisSasToken}";

        var analysis = new Analysis()
        {
            Title = title,
            Description = "",
            Details = htmlItemResponse,
            Url = analysisFileUrl,
            LogUrl = logFileUrl,
        };

        activity?.SetOutput(analysis);
        return analysis;
    }
    public async Task<Analysis> GenerateFooter(string logContent, string title, IDictionary<string, object> placeholders)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logContent, title, placeholders });

        var openAiConfig = openAiOptions.Value;
        var client = azureOpenAiClient.GetChatClient(openAiConfig.ChatModel); logger.LogDebug($"var client = azureOpenAiClient.GetChatClient({openAiConfig.ChatModel});");

        var timeInformation = placeholders.TryGetValue("timeInformation", out var timeInformationObj) ? (TimeInformation)timeInformationObj : new TimeInformation();
        var nowOffsetUtc = timeInformation.UtcNow;

        List<PromptChatMessage>? messages = null;
        var promptConfig = promptOptions.Value;
        var templateFolder = promptConfig.TemplateFolder;
        var templateName = "02.08 - Footer";
        var templateFileName = string.IsNullOrEmpty(templateFolder) ? $"{templateName}.template.html" : $"{templateFolder}\\{templateName}.template.html";
        string analysisFileName = $"{templateName}";

        var blobStorageConfig = this.blobStorageOptions.Value;
        var blobServiceClient = new BlobServiceClient(blobStorageConfig.BlobStorageConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient("analysis");
        var analysisSasToken = containerClient.GenerateSasUri(BlobContainerSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1)).Query;
        logger.LogDebug("analysisSasToken: {analysisSasToken}", analysisSasToken);

        string folderNamePrefix = $"{nowOffsetUtc.DateTime:yyyyMMdd HHmm} - ";
        string folderName = $"{folderNamePrefix}{title}";
        string logFileName = $"{folderNamePrefix}LogStream";
        var azureResourcesInformation = this.azureResourcesOptions.Value;
        var devopsInformation = this.devopsOptions.Value;
        var httpInformation = this.httpOptions.Value;
        var azureAdInformation = this.azureAdOptions.Value;
        var otherInformation = placeholders.TryGetValue("otherInformation", out var otherInformationObj) ? (Dictionary<string, object?>)otherInformationObj : new Dictionary<string, object?>();
        if (!otherInformation.ContainsKey("analysisSasToken")) { otherInformation.Add("analysisSasToken", analysisSasToken); }
        if (!otherInformation.ContainsKey("folderNamePrefix")) { otherInformation.Add("folderNamePrefix", folderNamePrefix); }
        if (!otherInformation.ContainsKey("logFileName")) { otherInformation.Add("logFileName", logFileName); }

        var htmlTemplateFileName = string.IsNullOrEmpty(templateFolder) ? $"{templateName}Template.html" : $"{templateFolder}\\{templateName}Template.html";
        var htmlTemplateContent = File.ReadAllText(templateFileName);
        var htmlItemResponse = PromptReplacePlaceholders(htmlTemplateContent, placeholders);

        var analysisBlobClient = containerClient.GetBlobClient($"{folderName}/{analysisFileName}.htm");
        using var analysisStream = new MemoryStream(Encoding.UTF8.GetBytes(htmlItemResponse));
        await analysisBlobClient.UploadAsync(analysisStream, overwrite: true);

        if (!otherInformation.ContainsKey(templateName)) { otherInformation.Add(templateName, htmlItemResponse); }

        var analysisFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{analysisFileName}.htm{analysisSasToken}";
        var logFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{logFileName}.log{analysisSasToken}";

        var analysis = new Analysis()
        {
            Title = title,
            Description = "",
            Details = htmlItemResponse,
            Url = analysisFileUrl,
            LogUrl = logFileUrl,
        };

        activity?.SetOutput(analysis);
        return analysis;
    }
    public async Task<Analysis> GenerateDetails(string logContent, string title, IDictionary<string, object> placeholders)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logContent, title, placeholders });

        var openAiConfig = openAiOptions.Value;
        var client = azureOpenAiClient.GetChatClient(openAiConfig.ChatModel); logger.LogDebug($"var client = azureOpenAiClient.GetChatClient({openAiConfig.ChatModel});");

        var timeInformation = placeholders.TryGetValue("timeInformation", out var timeInformationObj) ? (TimeInformation)timeInformationObj : new TimeInformation();
        var nowOffsetUtc = timeInformation.UtcNow;

        List<PromptChatMessage>? messages = null;
        var promptConfig = promptOptions.Value;
        var templateFolder = promptConfig.TemplateFolder;
        var templateName = "02.06 - ApplicationFlowDetails";
        var promptFileName = string.IsNullOrEmpty(templateFolder) ? $"{templateName}.prompt.yaml" : $"{templateFolder}\\{templateName}.prompt.yaml";

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

        string folderNamePrefix = $"{nowOffsetUtc.DateTime:yyyyMMdd HHmm} - ";
        string logFileName = $"{folderNamePrefix}LogStream";
        var azureResourcesInformation = this.azureResourcesOptions.Value;
        var devopsInformation = this.devopsOptions.Value;
        var httpInformation = this.httpOptions.Value;
        var azureAdInformation = this.azureAdOptions.Value;

        var otherInformation = placeholders.TryGetValue("otherInformation", out var otherInformationObj) ? (Dictionary<string, object?>)otherInformationObj : new Dictionary<string, object?>();
        if (!otherInformation.ContainsKey("analysisSasToken")) { otherInformation.Add("analysisSasToken", analysisSasToken); }
        if (!otherInformation.ContainsKey("folderNamePrefix")) { otherInformation.Add("folderNamePrefix", folderNamePrefix); }
        if (!otherInformation.ContainsKey("logFileName")) { otherInformation.Add("logFileName", logFileName); }


        List<ChatMessage> chatMessages = new();
        foreach (var messageObject in yamlObject)
        {
            var message = messageObject as IDictionary<object, object>;
            var requestHeaders = httpInformation.Headers;
            message["Value"] = PromptReplacePlaceholders(message["Value"] as string, placeholders);

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
            var actualPromptPath = $"{actualPromptFolder}\\{templateName}.prompt.actual.yaml";
            logger.LogDebug("actualPromptPath: {actualPromptPath}", actualPromptPath);
            await File.WriteAllTextAsync(actualPromptPath, actualPrompt); logger.LogDebug($"await File.WriteAllTextAsync({actualPromptPath}, actualPrompt);");
        }

        logger.LogDebug($"before client.CompleteChatAsync({chatMessages});");
        var response = await client.CompleteChatAsync(chatMessages);
        logger.LogDebug($"{response} = await client.CompleteChatAsync({chatMessages});");
        var ret = response.Value.Content?.FirstOrDefault()?.Text ?? "";

        var markdownContentPattern = "```html(.*)```";
        var match = Regex.Match(ret, markdownContentPattern, RegexOptions.Singleline);
        var htmlItemResponse = match.Groups[1].Value?.Trim();
        if (string.IsNullOrEmpty(htmlItemResponse)) { htmlItemResponse = ret; }

        //var doc = new HtmlDocument();
        //doc.LoadHtml(itemHtmlResponse);
        //var titleNode = doc.DocumentNode.SelectSingleNode("/section/h1");
        //var title = titleNode.InnerText.Trim();

        string folderName = $"{folderNamePrefix}{title}";

        string analysisFileName = $"{templateName}";
        var analysisBlobClient = containerClient.GetBlobClient($"{folderName}/{analysisFileName}.htm");
        using var analysisStream = new MemoryStream(Encoding.UTF8.GetBytes(htmlItemResponse));
        await analysisBlobClient.UploadAsync(analysisStream, overwrite: true);

        if (!otherInformation.ContainsKey(templateName)) { otherInformation.Add(templateName, htmlItemResponse); }

        var logBlobClientLog = containerClient.GetBlobClient($"{folderName}/{logFileName}.log");
        using var logStream = new MemoryStream(Encoding.UTF8.GetBytes(logContent));
        await logBlobClientLog.UploadAsync(logStream, overwrite: true);


        var analysisFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{analysisFileName}.htm{analysisSasToken}";
        var logFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{logFileName}.log{analysisSasToken}";

        var analysis = new Analysis()
        {
            Title = title,
            Description = "",
            Details = htmlItemResponse,
            Url = analysisFileUrl,
            LogUrl = logFileUrl,
        };

        activity?.SetOutput(analysis);
        return analysis;
    }
    public async Task<Analysis> GeneratePerformanceAnalysis(string logContent, string title, IDictionary<string, object> placeholders)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logContent, title, placeholders });

        var openAiConfig = openAiOptions.Value;
        var client = azureOpenAiClient.GetChatClient(openAiConfig.ChatModel); logger.LogDebug($"var client = azureOpenAiClient.GetChatClient({openAiConfig.ChatModel});");

        var timeInformation = placeholders.TryGetValue("timeInformation", out var timeInformationObj) ? (TimeInformation)timeInformationObj : new TimeInformation();
        var nowOffsetUtc = timeInformation.UtcNow;

        List<PromptChatMessage>? messages = null;
        var promptConfig = promptOptions.Value;
        var templateFolder = promptConfig.TemplateFolder;
        var templateName = "02.05 - GeneratePerformanceAnalysis";
        var promptFileName = string.IsNullOrEmpty(templateFolder) ? $"{templateName}.prompt.yaml" : $"{templateFolder}\\{templateName}.prompt.yaml";

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

        string folderNamePrefix = $"{nowOffsetUtc.DateTime:yyyyMMdd HHmm} - ";
        string logFileName = $"{folderNamePrefix}LogStream";
        var azureResourcesInformation = this.azureResourcesOptions.Value;
        var devopsInformation = this.devopsOptions.Value;
        var httpInformation = this.httpOptions.Value;
        var azureAdInformation = this.azureAdOptions.Value;

        var otherInformation = placeholders.TryGetValue("otherInformation", out var otherInformationObj) ? (Dictionary<string, object?>)otherInformationObj : new Dictionary<string, object?>();
        if (!otherInformation.ContainsKey("analysisSasToken")) { otherInformation.Add("analysisSasToken", analysisSasToken); }
        if (!otherInformation.ContainsKey("folderNamePrefix")) { otherInformation.Add("folderNamePrefix", folderNamePrefix); }
        if (!otherInformation.ContainsKey("logFileName")) { otherInformation.Add("logFileName", logFileName); }


        List<ChatMessage> chatMessages = new();
        foreach (var messageObject in yamlObject)
        {
            var message = messageObject as IDictionary<object, object>;
            var requestHeaders = httpInformation.Headers;
            message["Value"] = PromptReplacePlaceholders(message["Value"] as string, placeholders);

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
            var actualPromptPath = $"{actualPromptFolder}\\{templateName}.prompt.actual.yaml";
            logger.LogDebug("actualPromptPath: {actualPromptPath}", actualPromptPath);
            await File.WriteAllTextAsync(actualPromptPath, actualPrompt); logger.LogDebug($"await File.WriteAllTextAsync({actualPromptPath}, actualPrompt);");
        }

        logger.LogDebug($"before client.CompleteChatAsync({chatMessages});");
        var response = await client.CompleteChatAsync(chatMessages);
        logger.LogDebug($"{response} = await client.CompleteChatAsync({chatMessages});");
        var ret = response.Value.Content?.FirstOrDefault()?.Text ?? "";

        var markdownContentPattern = "```html(.*)```";
        var match = Regex.Match(ret, markdownContentPattern, RegexOptions.Singleline);
        var htmlItemResponse = match.Groups[1].Value?.Trim();
        if (string.IsNullOrEmpty(htmlItemResponse)) { htmlItemResponse = ret; }

        //var doc = new HtmlDocument();
        //doc.LoadHtml(itemHtmlResponse);
        //var titleNode = doc.DocumentNode.SelectSingleNode("/section/h1");
        //var title = titleNode.InnerText.Trim();

        string folderName = $"{folderNamePrefix}{title}";

        string analysisFileName = $"{templateName}";
        var analysisBlobClient = containerClient.GetBlobClient($"{folderName}/{analysisFileName}.htm");
        using var analysisStream = new MemoryStream(Encoding.UTF8.GetBytes(htmlItemResponse));
        await analysisBlobClient.UploadAsync(analysisStream, overwrite: true);

        if (!otherInformation.ContainsKey(templateName)) { otherInformation.Add(templateName, htmlItemResponse); }

        var logBlobClientLog = containerClient.GetBlobClient($"{folderName}/{logFileName}.log");
        using var logStream = new MemoryStream(Encoding.UTF8.GetBytes(logContent));
        await logBlobClientLog.UploadAsync(logStream, overwrite: true);


        var analysisFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{analysisFileName}.htm{analysisSasToken}";
        var logFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{logFileName}.log{analysisSasToken}";

        var analysis = new Analysis()
        {
            Title = title,
            Description = "",
            Details = htmlItemResponse,
            Url = analysisFileUrl,
            LogUrl = logFileUrl,
        };

        activity?.SetOutput(analysis);
        return analysis;
    }
    public async Task<Analysis> ComposeAnalysis(string logContent, string title, IDictionary<string, object> placeholders)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logContent, title, placeholders });

        var openAiConfig = openAiOptions.Value;
        var client = azureOpenAiClient.GetChatClient(openAiConfig.ChatModel); logger.LogDebug($"var client = azureOpenAiClient.GetChatClient({openAiConfig.ChatModel});");

        var timeInformation = placeholders.TryGetValue("timeInformation", out var timeInformationObj) ? (TimeInformation)timeInformationObj : new TimeInformation();
        var nowOffsetUtc = timeInformation.UtcNow;

        // load the html template 00.01 - FullAnalysisTemplate.html
        var promptConfig = promptOptions.Value;
        var templateFolder = promptConfig.TemplateFolder;
        var templateName = "00.01 - FullAnalysis";
        var templateFileName = string.IsNullOrEmpty(templateFolder) ? $"{templateName}.template.html" : $"{templateFolder}\\{templateName}.template.html";
        string analysisFileName = $"{templateName}";

        var blobStorageConfig = this.blobStorageOptions.Value;
        var blobServiceClient = new BlobServiceClient(blobStorageConfig.BlobStorageConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient("analysis");
        var analysisSasToken = containerClient.GenerateSasUri(BlobContainerSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1)).Query;
        logger.LogDebug("analysisSasToken: {analysisSasToken}", analysisSasToken);

        string folderNamePrefix = $"{nowOffsetUtc.DateTime:yyyyMMdd HHmm} - ";
        string folderName = $"{folderNamePrefix}{title}";
        string logFileName = $"{folderNamePrefix}LogStream";
        var azureResourcesInformation = this.azureResourcesOptions.Value;
        var devopsInformation = this.devopsOptions.Value;
        var httpInformation = this.httpOptions.Value;
        var azureAdInformation = this.azureAdOptions.Value;
        var otherInformation = placeholders.TryGetValue("otherInformation", out var otherInformationObj) ? (Dictionary<string, object?>)otherInformationObj : new Dictionary<string, object?>();
        if (!otherInformation.ContainsKey("analysisSasToken")) { otherInformation.Add("analysisSasToken", analysisSasToken); }
        if (!otherInformation.ContainsKey("folderNamePrefix")) { otherInformation.Add("folderNamePrefix", folderNamePrefix); }
        if (!otherInformation.ContainsKey("logFileName")) { otherInformation.Add("logFileName", logFileName); }

        var htmlTemplateFileName = string.IsNullOrEmpty(templateFolder) ? $"{templateName}Template.html" : $"{templateFolder}\\{templateName}Template.html";
        var htmlTemplateContent = File.ReadAllText(templateFileName);
        var htmlItemResponse = PromptReplacePlaceholders(htmlTemplateContent, placeholders);

        var analysisBlobClient = containerClient.GetBlobClient($"{folderName}/{analysisFileName}.htm");
        using var analysisStream = new MemoryStream(Encoding.UTF8.GetBytes(htmlItemResponse));
        await analysisBlobClient.UploadAsync(analysisStream, overwrite: true);

        //if (!otherInformation.ContainsKey(templateName)) { otherInformation.Add(templateName, htmlItemResponse); }

        var analysisFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{analysisFileName}.htm{analysisSasToken}";
        var logFileUrl = $"{containerClient.Uri.AbsoluteUri}/{folderName}/{logFileName}.log{analysisSasToken}";

        var analysis = new Analysis()
        {
            Title = title,
            Description = "",
            Details = htmlItemResponse,
            Url = analysisFileUrl,
            LogUrl = logFileUrl,
        };

        activity?.SetOutput(analysis);
        return analysis;
    }



    private Dictionary<string, object?> GetInferredPlaceholders(string itemHtmlResponse)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(itemHtmlResponse);

        var paramDic = new Dictionary<string, object?>();

        var rows = doc.DocumentNode.SelectNodes("//div[@class='table-row-auto']");
        if (rows != null)
        {
            foreach (var row in rows)
            {
                var nameNode = row.SelectSingleNode(".//div[@class='table-cell param-name']");
                var valueNode = row.SelectSingleNode(".//div[@class='table-cell param-value']");

                if (nameNode != null && valueNode != null)
                {
                    var paramName = nameNode.InnerText.Trim();
                    var paramValue = valueNode.InnerText.Trim();
                    if (!paramDic.ContainsKey(paramName))
                    {
                        paramDic.Add(paramName, paramValue);
                    }
                    else
                    {
                        var currentValue = paramDic[paramName];
                        paramDic[paramName] = $"{currentValue}\r\n{paramValue}";
                    }
                }
            }
        }

        return paramDic;
    }
    private string PromptReplacePlaceholders(string message, object variables)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { message = message?.GetLogString(), variables });
        if (variables == null) { return message; }

        var variablesDic = new Dictionary<string, object?>();
        foreach (var prop in variables.GetType().GetProperties())
        {
            variablesDic.Add(prop.Name, prop.GetValue(variables));
        }
        message = PromptReplacePlaceholders(message, variablesDic);

        activity?.SetOutput(message?.GetLogString());
        return message;
    }
    private string PromptReplacePlaceholders(string message, IDictionary<string, object?> variables)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { message = message?.GetLogString(), variables });
        if (variables == null) { return message; }
        if (string.IsNullOrEmpty(message)) { return message; }

        foreach (var placeholderName in variables.Keys)
        {
            var placeholderValue = variables[placeholderName];
            if (placeholderValue == null) { continue; }
            var placeholderType = placeholderValue.GetType();
            if (placeholderType.IsPrimitive || placeholderType == typeof(decimal) || placeholderType == typeof(string) || placeholderType == typeof(DateTime) || placeholderType == typeof(DateTimeOffset))
            {
                message = message.Replace($"{{{{{placeholderName}}}}}", placeholderValue.ToString());
            }
            else if (placeholderValue is IDictionary<string, object?>)
            {
                var valueDictionary = placeholderValue as IDictionary<string, object?>;
                foreach (var item in valueDictionary!)
                {
                    message = PromptReplacePlaceholders(message, valueDictionary);
                }
            }
            else if (IsEnumerableButNotPrimitive(placeholderValue))
            {
                var valueProperties = placeholderValue.GetType().GetProperties().Where(p => p.GetIndexParameters().Length == 0);
                foreach (var itemProp in valueProperties)
                {
                    object? itemValue = null;
                    try { itemValue = itemProp.GetValue(placeholderValue); } catch { continue; }
                    message = message.Replace($"{{{{{placeholderName}.{itemProp.Name}}}}}", itemValue?.ToString());
                }

                var start = "{{#each {{propName}}}}".Replace("{{propName}}", placeholderName);
                var end = "{{/each}}";

                var pattern = $"{start}(.*?){end}";
                var match = Regex.Match(message, pattern, RegexOptions.Singleline);
                var itemHtmlTemplate = match.Groups[0].Value;
                if (match.Groups == null || match.Groups.Count == 0 || string.IsNullOrEmpty(itemHtmlTemplate)) { continue; }

                itemHtmlTemplate = itemHtmlTemplate.Replace(start, "");
                itemHtmlTemplate = itemHtmlTemplate.Replace(end, "");
                var itemsText = new List<string>();
                foreach (var item in (placeholderValue as IEnumerable)!)
                {
                    var dic = new Dictionary<string, object?>(variables);
                    dic.Remove(placeholderName);
                    dic.Add(placeholderName.TrimEnd('s'), item);
                    var workItemHtml = PromptReplacePlaceholders(itemHtmlTemplate, dic);
                    itemsText.Add(workItemHtml.Trim());
                }
                message = Regex.Replace(message, pattern, string.Join("\r\n", itemsText), RegexOptions.Singleline);
            }
            else
            {
                var itemProperties = placeholderValue.GetType().GetProperties().Where(p => p.GetIndexParameters().Length == 0);
                foreach (var itemProp in itemProperties)
                {
                    object? itemValue = null;
                    try { itemValue = itemProp.GetValue(placeholderValue); } catch { continue; }

                    if (itemValue == null) { message = message.Replace($"{{{{{placeholderName}.{itemProp.Name}}}}}", null); continue; }

                    var type = itemValue.GetType();
                    if (type.IsPrimitive || type == typeof(decimal) || type == typeof(string) || type == typeof(DateTime) || type == typeof(DateTimeOffset))
                    {
                        message = message.Replace($"{{{{{placeholderName}.{itemProp.Name}}}}}", itemValue?.ToString());
                    }
                    else if (itemValue is IDictionary<string, object?>)
                    {
                        message = PromptReplacePlaceholders(message, itemValue as IDictionary<string, object?>);
                    }
                }
            }
        }

        activity?.SetOutput(message?.GetLogString());
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
