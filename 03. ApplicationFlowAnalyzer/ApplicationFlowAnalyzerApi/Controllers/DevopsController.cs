using Diginsight.Diagnostics;
using DiginsightCopilotApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiginsightCopilotApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DevopsController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<DevopsController> logger;

        public DevopsController(ILogger<DevopsController> logger)
        {
            this.logger = logger;
            using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logger });
        }

        [HttpGet(Name = "Get")]
        public IEnumerable<Analysis> Get()
        {
            using var activity = Observability.ActivitySource.StartMethodActivity(logger);

            return Enumerable.Range(1, 5).Select(index => new Analysis
            {
                Date = DateTime.Now,
                Title = "Sample Devops for application flow",
                Description = "Sample Devops description for application flow",
                Details = "Sample Devops details for application flow."
            })
            .ToArray();
        }

        // get BuildWorkitems
        // get BuildChanges 
        // get Method Source
        // get Class Source
        // get Assembly Source

    }
}
