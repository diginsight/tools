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
            using var activity = Observability.ActivitySource.StartMethodActivity(logger, new {  });

            using var reader = new StreamReader(Request.Body);
            var logContent = await reader.ReadToEndAsync();

            var result = await this.openAiService.GenerateSummary(logContent, 0, null!, null!);


            // Process the log content as needed
            logger.LogDebug("logContent:\r\n{logContent}");


            return result; // Ok()

        }


    }
}
