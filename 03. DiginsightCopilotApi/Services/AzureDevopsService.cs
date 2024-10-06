using Diginsight.Diagnostics;
using DiginsightCopilotApi.Abstractions;
using DiginsightCopilotApi.Configuration;
using DiginsightCopilotApi.Models;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.CommandLine;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace DiginsightCopilotApi.Services;

public class AzureDevopsService : IAzureDevopsService
{
    private readonly Uri devopsUri;
    private readonly VssBasicCredential credentials;
    private readonly ILogger<AzureDevopsService> logger;
    private readonly AzureDevopsConfig config;

    public AzureDevopsService(IOptions<AzureDevopsConfig> options, ILogger<AzureDevopsService> logger)
    {
        this.devopsUri = new Uri("https://dev.azure.com/" + options.Value.OrgName);
        credentials = new VssBasicCredential(string.Empty, options.Value.PAT);
        this.logger = logger;
        config = options.Value;
    }

    public async Task<WorkItem?> GetWorkItemAsync(int id)
    {
        WorkItem? result = null;

        try
        {
            using var httpClient = new WorkItemTrackingHttpClient(this.devopsUri, this.credentials);

            var fields = new[] { "System.Id", "System.Title", "System.State", "System.Description" };
            result = await httpClient.GetWorkItemAsync(id).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error getting work items by build id");
        }
        return result;
    }

    public async Task<IEnumerable<string>> GetApproversAsync()
    {
        using var connection = new VssConnection(this.devopsUri, this.credentials);

        using var projectHttpClient = connection.GetClient<ProjectHttpClient>();
        using var taskAgentHttpClient = connection.GetClient<TaskAgentHttpClient>();
        var project = await projectHttpClient.GetProject(this.config.Project);
        var environments = await taskAgentHttpClient.GetEnvironmentsAsync(project.Id);
        var environment = environments.FirstOrDefault();
        var environmentId = environment?.Id ?? 0;

        var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($":{config.PAT}")));
        var checkConfigurationsUrl = $"https://dev.azure.com/{this.config.OrgName}/{this.config.Project}/_apis/pipelines/checks/configurations?$expand=settings&resourceType=environment&resourceId=1&api-version=7.2-preview.1";
        var checkConfigurationsResponse = await client.GetAsync(checkConfigurationsUrl);
        checkConfigurationsResponse.EnsureSuccessStatusCode();
        var responseBody = await checkConfigurationsResponse.Content.ReadAsStringAsync();
        var approvalChecks = JsonConvert.DeserializeObject<Root>(responseBody);

        var checksValue = approvalChecks?.value.FirstOrDefault();
        var approvers = checksValue?.settings?.approvers;
        return approvers?.Select(x => x.uniqueName) ?? [];
        // CODE FOR APPROVERS FETCH
    }


    public async Task<IEnumerable<WorkItem>> GetWorkItemsByBuildIdAsync(int buildId, string project)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { buildId });

        var result = new List<WorkItem>();

        try
        {
            var devopsUri = new Uri("https://dev.azure.com/" + this.config.OrgName);
            var credentials = new VssBasicCredential(string.Empty, this.config.PAT);
            var connection = new VssConnection(this.devopsUri, credentials);

            var buildClient = connection.GetClient<BuildHttpClient>();
            var wiclient = connection.GetClient<WorkItemTrackingHttpClient>();

            var p = connection.GetClient<PipelinesHttpClient>();

            var projectName = project ?? this.config.Project;
            var buildByID = await buildClient.GetBuildAsync(project: projectName, buildId: buildId);
            var workItems = await buildClient.GetBuildWorkItemsRefsAsync(project: projectName, buildId: buildId);
            var workItemIds = workItems.Select(wi => int.Parse(wi.Id));
            if (workItemIds.Count() > 0)
            {
                result = await wiclient.GetWorkItemsAsync(workItemIds);
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error getting work items by build id");
        }
        return result;

    }

    public async Task<IEnumerable<Change>> GetChangesByBuildIdAsync(int buildId)
    {
        var result = new List<Change>();

        try
        {
            var connection = new VssConnection(this.devopsUri, credentials);
            var buildClient = connection.GetClient<BuildHttpClient>();
            return await buildClient.GetBuildChangesAsync(project: this.config.Project, buildId: buildId);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error getting work items by build id");
        }
        return result;
    }

    /// <summary>
    ///     Execute a WIQL (Work Item Query Language) query to return a list of open bugs.
    /// </summary>
    /// <param name="project">The name of your project within your organization.</param>
    /// <returns>A list of <see cref="WorkItem"/> objects representing all the open bugs.</returns>
    public async Task<IList<WorkItem>> QueryOpenBugs()
    {
        // create a wiql object and build our query
        var wiql = new Wiql()
        {
            // NOTE: Even if other columns are specified, only the ID & URL are available in the WorkItemReference
            Query = "Select [Id] " +
                    "From WorkItems " +
                    "Where [Work Item Type] = 'Bug' " +
                    "And [System.TeamProject] = '" + this.config.Project + "' " +
                    "And [System.State] <> 'Closed' " +
                    "Order By [State] Asc, [Changed Date] Desc",
        };

        // create instance of work item tracking http client
        using (var httpClient = new WorkItemTrackingHttpClient(this.devopsUri, credentials))
        {
            // execute the query to get the list of work items in the results
            var result = await httpClient.QueryByWiqlAsync(wiql).ConfigureAwait(false);
            var ids = result.WorkItems.Select(item => item.Id).ToArray();

            // some error handling
            if (ids.Length == 0)
            {
                return Array.Empty<WorkItem>();
            }

            // build a list of the fields we want to see
            var fields = new[] { "System.Id", "System.Title", "System.State" };

            // get work items for the ids found in query
            return await httpClient.GetWorkItemsAsync(ids, fields, result.AsOf).ConfigureAwait(false);
        }
    }
}
