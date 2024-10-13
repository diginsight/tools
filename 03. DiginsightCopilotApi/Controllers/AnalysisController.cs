using Diginsight.Diagnostics;
using DiginsightCopilotApi.Abstractions;
using DiginsightCopilotApi.Configuration;
using DiginsightCopilotApi.Models;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.Build.WebApi;
using MimeKit;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace DiginsightCopilotApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly ILogger<AnalysisController> logger;
        private readonly IOptions<AzureDevopsConfig> devopsOptions;
        private readonly IOptions<HttpContextConfig> httpOptions;

        private readonly IAzureDevopsService azureDevopsService;
        private readonly ISummaryService openAiService;
        private readonly IEmailService emailClient;

        public AnalysisController(
            ILogger<AnalysisController> logger,
            IOptions<AzureDevopsConfig> devopsOptions,
            IOptions<HttpContextConfig> httpOptions,
            IAzureDevopsService azureDevopsService,
            ISummaryService openAiService,
            IEmailService emailClient
            )
        {
            this.logger = logger;
            using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logger });

            this.devopsOptions = devopsOptions;
            this.httpOptions = httpOptions;
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

            // AzureMonitorConnectionString: InstrumentationKey=efc770fb-0443-4268-ba9f-a7bf293a68c8;IngestionEndpoint=https://northeurope-0.in.applicationinsights.azure.com/;LiveEndpoint=https://northeurope.livediagnostics.monitor.azure.com/;ApplicationId=abf064f1-a9aa-45c6-adc7-42b2684bcb5c
            var azureMonitorConnectionStringPattern = @"AzureMonitorConnectionString: InstrumentationKey=(.*);IngestionEndpoint=(.*)/;LiveEndpoint=(.*);ApplicationId=(.*)";
            var azureMonitorConnectionStringMatch = Regex.Match(logContent, azureMonitorConnectionStringPattern);
            if (azureMonitorConnectionStringMatch.Success)
            {
                var azureMonitorConnectionString = azureMonitorConnectionStringMatch.Value;
                var instrumentationKey = azureMonitorConnectionStringMatch.Groups[1].Value; 
                var ingestionEndpoint = azureMonitorConnectionStringMatch.Groups[2].Value; 
                var liveEndpoint =  azureMonitorConnectionStringMatch.Groups[3].Value;
                var applicationId = azureMonitorConnectionStringMatch.Groups[4].Value;
                logger.LogDebug("instrumentationKey: {instrumentationKey}, ingestionEndpoint: {ingestionEndpoint}, liveEndpoint: {liveEndpoint}, applicationId: {applicationId}", instrumentationKey, ingestionEndpoint, liveEndpoint, applicationId);

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
                        devopsOptions.Value.Project = buildNumber!;
                    }
                    else if (metadataName == "Build.SourceBranch")
                    {
                        var sourceBranch = metadataValue;
                        logger.LogDebug("sourceBranch: {sourceBranch}", sourceBranch);
                        devopsOptions.Value.Project = sourceBranch!;
                    }
                    else if (metadataName == "Build.Repository")
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
    }
}
