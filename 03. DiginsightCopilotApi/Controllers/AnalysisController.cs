using Diginsight.Diagnostics;
using DiginsightCopilotApi.Abstractions;
using DiginsightCopilotApi.Configuration;
using DiginsightCopilotApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.Build.WebApi;
using System.Text.RegularExpressions;

namespace DiginsightCopilotApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly ILogger<AnalysisController> logger;
        private readonly IOptions<AzureDevopsConfig> options;
        private readonly IAzureDevopsService azureDevopsService;
        private readonly ISummaryService openAiService;
        private readonly IEmailService emailClient;

        public AnalysisController(
            ILogger<AnalysisController> logger,
            IOptions<AzureDevopsConfig> options,
            IAzureDevopsService azureDevopsService,
            ISummaryService openAiService,
            IEmailService emailClient
            )
        {
            this.logger = logger;
            using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logger });

            this.options = options;
            this.azureDevopsService = azureDevopsService;
            this.openAiService = openAiService;
            this.emailClient = emailClient;
        }

        // ...

        [HttpPost("GenerateAnalysis")]
        [ActionName("GenerateAnalysis")]
        public async Task<string> GenerateAnalysisAsync()
        {
            using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { });

            using var reader = new StreamReader(Request.Body);
            var logContent = await reader.ReadToEndAsync();

            // look for a row into logContent containing Build.BuildUri=vstfs:///Build/Build/<number>
            var buildId = 0; var devopsProject = string.Empty; var devopsRepository = string.Empty;
            var buildUriPattern = @"Metadata: .*Build\.BuildUri=vstfs:///Build/Build/(\d+)";
            var match = Regex.Match(logContent, buildUriPattern);
            if (match.Success)
            {
                buildId = int.Parse(match.Groups[1].Value);
                logger.LogInformation("ComposeSummaryAsync called with buildId: {buildId}", buildId);
            }

            // look for a row into logContent containing Metadata: <anychar>/System.TeamProject=<project>
            // get the teamproject name
            var projectPattern = @"Metadata: .*System\.TeamProject=(\w+)";
            match = Regex.Match(logContent, projectPattern);
            if (match.Success)
            {
                devopsProject = match.Groups[1].Value;
                logger.LogInformation("ComposeSummaryAsync called with project: {devopsProject}", devopsProject);
            }
            var projectRepositoryPattern = @"Metadata: .*Build\.Repository\.Name=(\w+)";
            match = Regex.Match(logContent, projectRepositoryPattern);
            if (match.Success)
            {
                devopsRepository = match.Groups[1].Value;
                logger.LogInformation("ComposeSummaryAsync called with repository: {devopsProject}", devopsRepository);
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

            options.Value.BuildID = buildId.ToString();
            options.Value.Project = devopsProject;
            options.Value.Repository = devopsRepository;

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
