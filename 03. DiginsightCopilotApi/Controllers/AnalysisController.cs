using Diginsight.Diagnostics;
using DiginsightCopilotApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiginsightCopilotApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalysisController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<AnalysisController> logger;

        public AnalysisController(ILogger<AnalysisController> logger)
        {
            this.logger = logger;
            using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logger });
        }

        [HttpGet(Name = "GetApplicationFlowAnalysis")]
        public IEnumerable<Analysis> Get()
        {
            using var activity = Observability.ActivitySource.StartMethodActivity(logger);

            return Enumerable.Range(1, 5).Select(index => new Analysis
            {
                Date = DateTime.Now,
                Title = "Sample analysis for application flow",
                Description = "Sample analysis description for application flow",
                Details = "Sample analysis details for application flow."
            })
            .ToArray();
        }

        [HttpPost("GenerateAnalysis")]
        [ActionName("GenerateAnalysis")]
        public async Task<string> GenerateAnalysisAsync()
        {
            using var activity = Observability.ActivitySource.StartMethodActivity(logger, new {  });

            using var reader = new StreamReader(Request.Body);
            var logContent = await reader.ReadToEndAsync();

            // Process the log content as needed
            logger.LogDebug("logContent:\r\n{logContent}");


            return logContent; // Ok()

        }


    }
}
