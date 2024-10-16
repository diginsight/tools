using Azure.Identity;
using Azure.ResourceManager.ResourceGraph.Models;
using Azure.ResourceManager;
using Diginsight.Diagnostics;
using DiginsightCopilotApi.Abstractions;
using DiginsightCopilotApi.Configuration;
using DiginsightCopilotApi.Models;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.Build.WebApi;
using MimeKit;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;
using Azure.ResourceManager.ResourceGraph;
using Azure.Core;

namespace DiginsightCopilotApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalysisController : ControllerBase
    {
        private static readonly string resourceGraphEndpoint = "https://management.azure.com/providers/Microsoft.ResourceGraph/resources?api-version=2022-10-01";
        private readonly ILogger<AnalysisController> logger;
        private readonly IOptions<AzureAdOptions> azureAdOptions;
        private readonly IOptions<AzureDevopsOptions> devopsOptions;
        private readonly IOptions<HttpContextOptions> httpOptions;
        private readonly IOptions<AzureResourcesOptions> azureResourcesOptions;

        private readonly IAzureDevopsService azureDevopsService;
        private readonly ISummaryService openAiService;
        private readonly IEmailService emailClient;

        public AnalysisController(
            ILogger<AnalysisController> logger,
            IOptions<AzureAdOptions> azureAdOptions,
            IOptions<AzureDevopsOptions> devopsOptions,
            IOptions<HttpContextOptions> httpOptions,
            IOptions<AzureResourcesOptions> azureResourcesOptions,
            IAzureDevopsService azureDevopsService,
            ISummaryService openAiService,
            IEmailService emailClient
            )
        {
            this.logger = logger;
            using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logger });

            this.azureAdOptions = azureAdOptions;
            this.devopsOptions = devopsOptions;
            this.httpOptions = httpOptions;
            this.azureResourcesOptions = azureResourcesOptions;
            this.azureDevopsService = azureDevopsService;
            this.openAiService = openAiService;
            this.emailClient = emailClient;
        }

        [HttpPost("GenerateAnalysis")]
        [ActionName("GenerateAnalysis")]
        public async Task<string> GenerateAnalysisAsync()
        {
            using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { });

            using var reader = new StreamReader(Request.Body);
            var logContent = await reader.ReadToEndAsync();

            var incomingRequestPattern = @"Incoming Request: (.*) (http.*)";
            var incomingRequestMatch = Regex.Match(logContent, incomingRequestPattern);
            if (incomingRequestMatch.Success)
            {
                var incomingRequest = incomingRequestMatch.Value;
                incomingRequest = incomingRequest.Substring("Incoming Request: ".Length);
                logger.LogInformation("incomingRequest: {incomingRequest}", incomingRequest);
                var httpMethod = incomingRequestMatch.Groups[1].Value;
                var httpUrl = incomingRequestMatch.Groups[2].Value;
                var incomingUri = new Uri(httpUrl);
                var incomingPath = incomingUri.AbsolutePath;
                var incomingQuery = incomingUri.Query;
                var incomingHost = incomingUri.Host;
                var incomingPort = incomingUri.Port;
                var incomingScheme = incomingUri.Scheme;
                var incomingAuthority = incomingUri.Authority;

                this.httpOptions.Value.Method = httpMethod;
                this.httpOptions.Value.Url = httpUrl;
                this.httpOptions.Value.Uri = incomingUri.AbsoluteUri;
                this.httpOptions.Value.Path = incomingPath;
                this.httpOptions.Value.Query = incomingQuery;
                this.httpOptions.Value.Host = incomingHost;
                this.httpOptions.Value.Port = incomingPort.ToString();
                this.httpOptions.Value.Scheme = incomingScheme;
                this.httpOptions.Value.Authority = incomingAuthority;
            }

            var traceIdPattern = @"DBUG ([0-9a-fA-F]{32})(.*)LandingCallMiddleware.InvokeAsync";
            var traceIdMatch = Regex.Match(logContent, traceIdPattern);
            if (traceIdMatch.Success)
            {
                var traceId = traceIdMatch.Groups[1].Value;
                logger.LogDebug("traceId: {traceId}", traceId);

                this.azureResourcesOptions.Value.ApplicationInsightTraceId = traceId;
            }

            var azureMonitorConnectionStringPattern = @"AzureMonitorConnectionString: InstrumentationKey=(.*);IngestionEndpoint=(.*)/;LiveEndpoint=(.*);ApplicationId=(.*)";
            var azureMonitorConnectionStringMatch = Regex.Match(logContent, azureMonitorConnectionStringPattern);
            if (azureMonitorConnectionStringMatch.Success)
            {
                var azureMonitorConnectionString = azureMonitorConnectionStringMatch.Value;
                var instrumentationKey = azureMonitorConnectionStringMatch.Groups[1].Value;
                var ingestionEndpoint = azureMonitorConnectionStringMatch.Groups[2].Value;
                var liveEndpoint = azureMonitorConnectionStringMatch.Groups[3].Value;
                var applicationId = azureMonitorConnectionStringMatch.Groups[4].Value;
                logger.LogDebug("instrumentationKey: {instrumentationKey}, ingestionEndpoint: {ingestionEndpoint}, liveEndpoint: {liveEndpoint}, applicationId: {applicationId}", instrumentationKey, ingestionEndpoint, liveEndpoint, applicationId);

                this.azureResourcesOptions.Value.AzureMonitorConnectionString = azureMonitorConnectionString;
                this.azureResourcesOptions.Value.InstrumentationKey = instrumentationKey;
                this.azureResourcesOptions.Value.IngestionEndpoint = ingestionEndpoint;
                this.azureResourcesOptions.Value.LiveEndpoint = liveEndpoint;
                this.azureResourcesOptions.Value.ApplicationId = applicationId;
                
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
            }

            var httpRequestHeaders = new List<HttpRequestHeader>();
            var incomingRequestHeaderPattern = @"Incoming Request Header: (.*) - (.*)";
            var incomingRequestHeaderMatch = Regex.Match(logContent, incomingRequestHeaderPattern);
            if (incomingRequestHeaderMatch != null)
            {
                foreach (Match match in Regex.Matches(logContent, incomingRequestHeaderPattern))
                {
                    var incomingRequestHeader = match.Value;
                    var headerName = match.Groups[1].Value?.Trim();
                    var headerValue = match.Groups[2].Value?.Trim();
                    logger.LogInformation("Request header '{headerName}': '{headerValue}'", headerName, headerValue);
                    httpRequestHeaders.Add(new HttpRequestHeader() { Name = headerName!, Value = headerValue! });

                    if (headerName == "Referer")
                    {
                        var referer = headerValue;
                        var refererUri = new Uri(referer);
                        this.httpOptions.Value.Referer = referer;
                        var refererHost = refererUri.Host;
                        this.httpOptions.Value.RefererHost = refererHost;
                        if (refererHost?.StartsWith("test") ?? false) { devopsOptions.Value.Environment = "Test"; }
                        else if (refererHost?.StartsWith("stage") ?? false) { devopsOptions.Value.Environment = "Stage"; }
                        else { devopsOptions.Value.Environment = "Production"; }
                    }
                }
            }
            this.httpOptions.Value.Headers = httpRequestHeaders;

            var httpConfig = this.httpOptions.Value;
            var first = $@"curl '{httpConfig.Scheme}://{httpConfig.Host}{httpConfig.Path}{httpConfig.Query}' \";
            var second = $@"-X '{httpConfig.Method}' \";
            var headerStrings = new List<string>();
            if (httpConfig.Headers != null)
            {
                foreach (var header in httpConfig.Headers)
                {
                    if (header.Name == "Authorization") { continue; }
                    headerStrings.Add($@"-H '{header.Name}: {header.Value}' \");
                }
            }
            var headers = string.Join("\r\n", headerStrings);
            // --data-raw ''
            var curl = $"""
                   {first}
                   {second}
                   {headers}
                   """.Trim();
            this.httpOptions.Value.Curl = curl;

            var buildId = 0; var devopsProject = "";
            var assemblyMetadata = new List<AssemblyMetadata>();
            var assemblyMetadataPattern = @"Assembly Metadata: AzurePipelines/(.*)=(.*)";
            var assemblyMetadataMatch = Regex.Match(logContent, incomingRequestHeaderPattern);
            if (assemblyMetadataMatch != null)
            {
                foreach (Match match in Regex.Matches(logContent, assemblyMetadataPattern))
                {
                    var assemblyMetadataItem = match.Value;
                    var metadataName = match.Groups[1].Value?.ToString()?.Trim();
                    var metadataValue = match.Groups[2].Value?.ToString()?.Trim();
                    assemblyMetadata.Add(new AssemblyMetadata() { Name = metadataName!, Value = metadataValue! });

                    if (metadataName == "Build.BuildUri")
                    {
                        var buildString = metadataValue;
                        var buildStringParts = buildString?.Split('/');
                        var buildIdString = buildStringParts?.Last();
                        buildId = !string.IsNullOrEmpty(buildIdString) ? int.Parse(buildIdString) : 0;
                        logger.LogDebug("buildId: {buildId}", buildId);
                        devopsOptions.Value.BuildID = buildId.ToString();
                    }
                    else if (metadataName == "System.TeamProject")
                    {
                        devopsProject = metadataValue;
                        logger.LogDebug("teamProject: {teamProject}", devopsProject);
                        devopsOptions.Value.Project = devopsProject!;
                    }
                    else if (metadataName == "Build.BuildNumber")
                    {
                        var buildNumber = metadataValue;
                        logger.LogDebug("buildNumber: {buildNumber}", buildNumber);
                        devopsOptions.Value.BuildNumber = buildNumber!;
                    }
                    else if (metadataName == "Build.SourceBranch")
                    {
                        var sourceBranch = metadataValue;
                        logger.LogDebug("sourceBranch: {sourceBranch}", sourceBranch);
                        devopsOptions.Value.Branch = sourceBranch!;
                    }
                    else if (metadataName == "Build.Repository.Name")
                    {
                        var devopsRepository = metadataValue;
                        logger.LogDebug("devopsRepository: {devopsRepository}", devopsRepository);
                        devopsOptions.Value.Repository = devopsRepository!;
                    }
                }
            }

            var workItemParams = new List<WorkItemParam>();
            var changeParams = new List<ChangeParam>();
            if (buildId > 0)
            {
                var workItems = await azureDevopsService.GetWorkItemsByBuildIdAsync(buildId, devopsProject);
                foreach (var workItem in workItems)
                {
                    if (workItem == null || workItem.Id == null) { continue; }

                    var id = workItem.Id.Value;
                    var title = workItem.Fields["System.Title"].ToString();
                    var description = workItem.Fields.TryGetValue("System.Description", out var descObj) ? descObj?.ToString() : null;
                    var acceptanceCriteria = workItem.Fields.TryGetValue("Microsoft.VSTS.Common.AcceptanceCriteria", out var accObj) ? accObj?.ToString() : null;
                    workItemParams.Add(new WorkItemParam
                    {
                        Id = id,
                        Title = title ?? string.Empty,
                        Description = description ?? string.Empty,
                        AcceptanceCriteria = acceptanceCriteria ?? string.Empty,
                    });
                }

                var changes = await azureDevopsService.GetChangesByBuildIdAsync(buildId);
                foreach (var change in changes)
                {
                    changeParams.Add(new ChangeParam
                    {
                        Id = change.Id,
                        Message = change.Message,
                        Timestamp = change.Timestamp,
                        DisplayUriAbsoluteUri = change.DisplayUri.AbsoluteUri,
                    });
                }

            }

            var analysis = await this.openAiService.GenerateSummary(logContent, buildId, workItemParams, changeParams, assemblyMetadata);

            // Add response header
            Response.Headers.Add("analysis-url", analysis.Url);
            Response.Headers.Add("log-url", analysis.LogUrl);

            // Process the log content as needed
            logger.LogDebug("logContent:\r\n{logContent}");

            return analysis.Details; // Ok()
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
    }
}
