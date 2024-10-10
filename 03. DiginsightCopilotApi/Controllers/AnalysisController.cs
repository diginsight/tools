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

            var incomingRequestPattern = @"Incoming Request: .* http.*";
            var incomingRequestMatch = Regex.Match(logContent, incomingRequestPattern);
            if (incomingRequestMatch.Success)
            {
                var incomingRequest = incomingRequestMatch.Value;
                incomingRequest = incomingRequest.Substring("Incoming Request: ".Length);
                logger.LogInformation("ComposeSummaryAsync called with incomingRequest: {incomingRequest}", incomingRequest);
                var incomingRequestSplit = incomingRequest.Split(' ');
                var httpMethod = incomingRequestSplit[0];
                var httpUrl = incomingRequestSplit[1];
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

            // 2024-10-01T12:11:26.872 …ingCallMiddleware.LandingCallMiddleware DBUG 21c964baaae1c39b551134a1ee6aead7 .077m        2   Incoming Request Header: Referer - https://test.developers.connect.abb.com/
            var incomingRequestHeaderPattern = @"Incoming Request Header: .*";
            var incomingRequestHeaderMatch = Regex.Match(logContent, incomingRequestHeaderPattern);
            // loop on all occurrences of 'Incoming Request Header: ' and log the 'Incoming Request Header' value
            if (incomingRequestHeaderMatch != null)
            {
                foreach (Match mtch in Regex.Matches(logContent, incomingRequestHeaderPattern))
                {
                    var incomingRequestHeader = mtch.Value;
                    incomingRequestHeader = incomingRequestHeader.Substring("Incoming Request Header: ".Length);
                    logger.LogDebug("incomingRequestHeader: {incomingRequestHeader}", incomingRequestHeader);

                    var incomingRequestHeaderSplit = incomingRequestHeader.Split('-');
                    var headerName = incomingRequestHeaderSplit[0]?.Trim() ?? "";
                    if (! headerName.Equals("Referer", StringComparison.OrdinalIgnoreCase)) continue;

                    var headerValue = incomingRequestHeaderSplit[1]?.Trim();
                    var referer = headerValue;
                    var refererUri = new Uri(referer);
                    this.httpOptions.Value.Referer = referer;
                    var refererHost = refererUri.Host;
                    this.httpOptions.Value.RefererHost = refererHost;
                    if (refererHost?.StartsWith("test") ?? false) { devopsOptions.Value.Environment = "Test"; }
                    else if (refererHost?.StartsWith("stage") ?? false) { devopsOptions.Value.Environment = "Stage"; }
                    else { devopsOptions.Value.Environment = "Production"; }
                    //if (referer?.StartsWith("stage") ?? false) { devopsOptions.Value.Environment = "Stage"; }
                }
            }

            var buildId = 0; var devopsProject = string.Empty; var devopsRepository = string.Empty;
            var buildUriPattern = @"Metadata: .*Build\.BuildUri=vstfs:///Build/Build/(\d+)";
            var match = Regex.Match(logContent, buildUriPattern);
            if (match.Success)
            {
                buildId = int.Parse(match.Groups[1].Value);
                logger.LogInformation("buildId: {buildId}", buildId);
                devopsOptions.Value.BuildID = buildId.ToString();
            }

            var projectPattern = @"Metadata: .*System\.TeamProject=(\w+)";
            match = Regex.Match(logContent, projectPattern);
            if (match.Success)
            {
                devopsProject = match.Groups[1].Value;
                logger.LogInformation("Project: {devopsProject}", devopsProject);
                devopsOptions.Value.Project = devopsProject;
            }

            var buildNumberPattern = @"Metadata: (.*?)Build\.BuildNumber=(.*)";
            match = Regex.Match(logContent, buildNumberPattern);
            if (match.Success)
            {
                var buildNumber = match.Groups[2].Value;
                logger.LogInformation("Project: {buildNumber}", buildNumber);
                devopsOptions.Value.BuildNumber = buildNumber;
            }
            //Build.SourceBranch
            var sourceBranchPattern = @"Metadata: (.*?)Build\.SourceBranch=(.*)";
            match = Regex.Match(logContent, sourceBranchPattern);
            if (match.Success)
            {
                var sourceBranch = match.Groups[2].Value;
                logger.LogInformation("Project: {sourceBranch}", sourceBranch);
                devopsOptions.Value.Branch = sourceBranch;
            }

            var projectRepositoryPattern = @"Metadata: (.*?)Build\.Repository\.Name=(.*)";
            match = Regex.Match(logContent, projectRepositoryPattern);
            if (match.Success)
            {
                devopsRepository = match.Groups[2].Value;
                logger.LogInformation("Repository: {devopsRepository}", devopsRepository);
                devopsOptions.Value.Repository = devopsRepository;
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

            var analysis = await this.openAiService.GenerateSummary(logContent, buildId, workItemParams, changeParams);

            // Add response header
            Response.Headers.Add("analysis-url", analysis.Url);
            Response.Headers.Add("log-url", analysis.LogUrl);

            // Process the log content as needed
            logger.LogDebug("logContent:\r\n{logContent}");

            return analysis.Details; // Ok()
        }
    }
}
