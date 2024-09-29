using DiginsightCopilotApi.Models;

namespace DiginsightCopilotApi.Abstractions;

public interface ISummaryService
{
    Task<string> GenerateSummary(string logContent, int buildId, IEnumerable<WorkItemParam> workItems, IEnumerable<ChangeParam> changes);
}