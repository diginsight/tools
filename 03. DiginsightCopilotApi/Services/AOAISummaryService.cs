using Azure.AI.OpenAI;
using Diginsight.Diagnostics;
using DiginsightCopilotApi.Abstractions;
using DiginsightCopilotApi.Configuration;
using DiginsightCopilotApi.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenAI.Chat;
using OpenAI;
using System.ClientModel;
using System.Text.RegularExpressions;
using System.Collections;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace DiginsightCopilotApi.Services;

public class AOAISummaryService : ISummaryService
{
    private readonly ILogger<AOAISummaryService> logger;
    private readonly AzureDevopsConfig devopsConfig;
    private AzureOpenAiConfig openAiConfig;
    private OpenAIClient openAiClient;
    private AzureOpenAIClient azureOpenAiClient;

    public AOAISummaryService(
        ILogger<AOAISummaryService> logger,
        IOptions<AzureOpenAiConfig> openAiOptions,
        IOptions<AzureDevopsConfig> devopsOptions)
    {
        this.logger = logger;
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { openAiOptions, devopsOptions });

        openAiConfig = openAiOptions.Value;
        azureOpenAiClient = new AzureOpenAIClient(new Uri(openAiConfig.Endpoint), new ApiKeyCredential(openAiOptions.Value.ApiKey));

        devopsConfig = devopsOptions.Value;
    }

    public async Task<string> GenerateSummary(string logContent, int buildId, IEnumerable<WorkItemParam> workItems, IEnumerable<ChangeParam> changes)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logContent, buildId, workItems, changes });

        var client = azureOpenAiClient.GetChatClient(openAiConfig.ChatModel); logger.LogDebug($"var client = azureOpenAiClient.GetChatClient({openAiConfig.ChatModel});");

        //List<PromptChatMessage>? messages = null;
        //var promptTemplate = File.ReadAllText("01.LogSummarize.prompt.json");
        //messages = JsonConvert.DeserializeObject<List<PromptChatMessage>>(promptTemplate);
        //if (messages == null) { throw new InvalidOperationException("Prompt template is not valid"); }

        List<PromptChatMessage>? messages = null;
        var promptYamlTemplate = File.ReadAllText("01.LogSummarize.prompt.yaml");
        var deserializer = new DeserializerBuilder()
                  .WithNamingConvention(new PascalCaseNamingConvention())
                  .Build();
        var yamlObject = deserializer.Deserialize(new StringReader(promptYamlTemplate)) as IList<object>;
        //var serializer = new SerializerBuilder()
        //    .JsonCompatible()
        //    .Build();
        //var json = serializer.Serialize(yamlObject);
        //messages = JsonConvert.DeserializeObject<List<PromptChatMessage>>(json);

        List<ChatMessage> chatMessages = new();
        foreach (var messageObject in yamlObject)
        {
            var message = messageObject as IDictionary<object, object>;
            message["Value"] = PromptReplacePlaceholders(message["Value"] as string, new { devopsConfig, logContent }); // , workItems, changes, buildId
            if (message["Type"].Equals("SystemChatMessage")) { chatMessages.Add(new SystemChatMessage(message["Value"] as string)); }
            else if (message["Type"].Equals("UserChatMessage")) { chatMessages.Add(new UserChatMessage(message["Value"] as string)); }

            //message.Value = PromptReplacePlaceholders(message.Value, new { devopsConfig, logContent }); // , workItems, changes, buildId
            //if (message.Type == "SystemChatMessage") { chatMessages.Add(new SystemChatMessage(message.Value)); }
            //else if (message.Type == "UserChatMessage") { chatMessages.Add(new UserChatMessage(message.Value)); }
        }

        logger.LogDebug($"before client.CompleteChatAsync({chatMessages});");

        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"; logger.LogDebug("isDevelopment: {isDevelopment}", isDevelopment);
        if (isDevelopment)
        {
            var actualPrompt = JsonConvert.SerializeObject(chatMessages);
            var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var applicationName = AppDomain.CurrentDomain.FriendlyName;
            var actualPromptFolder = $"{userProfilePath}\\{applicationName}";
            Directory.CreateDirectory(actualPromptFolder);
            var actualPromptPath = $"{actualPromptFolder}\\DevopsSummarize.prompt.actual.json";
            logger.LogDebug("actualPromptPath: {actualPromptPath}", actualPromptPath);
            await File.WriteAllTextAsync(actualPromptPath, actualPrompt); logger.LogDebug($"await File.WriteAllTextAsync({actualPromptPath}, actualPrompt);");
        }

        var response = await client.CompleteChatAsync(chatMessages);
        logger.LogDebug($"{response} = await client.CompleteChatAsync({chatMessages});");
        var ret = response.Value.Content?.FirstOrDefault()?.Text ?? "";

        activity?.SetOutput(ret);
        return ret;
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
            if (type.IsPrimitive || type == typeof(decimal) || type == typeof(string))
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
}
