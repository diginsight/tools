using Diginsight.Diagnostics;
using DiginsightCopilotApi.Abstractions;
using DiginsightCopilotApi.Configuration;
using DiginsightCopilotApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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

        [HttpPost("GenerateAnalysis")]
        [ActionName("GenerateAnalysis")]
        public async Task<string> GenerateAnalysisAsync()
        {
            using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { });

            using var reader = new StreamReader(Request.Body);
            var logContent = await reader.ReadToEndAsync();

            var buildId = 237401;
            logger.LogInformation("ComposeSummaryAsync called with buildId: {buildId}", buildId);
            List<WorkItemParam> workItemParams = new List<WorkItemParam>();
            var workItems = await azureDevopsService.GetWorkItemsByBuildIdAsync(buildId);

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

            List<ChangeParam> changeParams = new List<ChangeParam>();
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

            var result = await this.openAiService.GenerateSummary(logContent, buildId, workItemParams, changeParams);


            // Process the log content as needed
            logger.LogDebug("logContent:\r\n{logContent}");


            return result; // Ok()

        }


    }
}
