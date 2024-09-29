using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

namespace DiginsightCopilotApi.Abstractions;

public interface IAzureDevopsService
{
    Task<WorkItem?> GetWorkItemAsync(int id);
    Task<IEnumerable<WorkItem>> GetWorkItemsByBuildIdAsync(int buildId);
    Task<IEnumerable<Change>> GetChangesByBuildIdAsync(int buildId);
    Task<IList<WorkItem>> QueryOpenBugs();
    Task<IEnumerable<string>> GetApproversAsync();

}
