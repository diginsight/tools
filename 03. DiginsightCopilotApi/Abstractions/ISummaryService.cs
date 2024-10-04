using DiginsightCopilotApi.Models;

namespace DiginsightCopilotApi.Abstractions;

public interface ISummaryService
{
    Task<Analysis> GenerateSummary(string logContent, int buildId, IEnumerable<WorkItemParam> workItems, IEnumerable<ChangeParam> changes);
}