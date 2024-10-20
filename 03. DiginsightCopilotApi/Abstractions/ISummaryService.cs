using DiginsightCopilotApi.Models;

namespace DiginsightCopilotApi.Abstractions;

public interface ISummaryService
{
    Task<Analysis> GenerateTitle(string logContent, DateTimeOffset utcNow, int buildId, IEnumerable<WorkItemParam> workItems, IEnumerable<ChangeParam> changes, IEnumerable<AssemblyMetadata> assemblyMetadata);
    Task<Analysis> GenerateFullAnalysis(string logContent, DateTimeOffset utcNow, string title, int buildId, IEnumerable<WorkItemParam> workItems, IEnumerable<ChangeParam> changes, IEnumerable<AssemblyMetadata> assemblyMetadata);
    Task<Analysis> InferPlaceholders(string logContent, DateTimeOffset utcNow, string title, int buildId, IEnumerable<WorkItemParam> workItems, IEnumerable<ChangeParam> changes, IEnumerable<AssemblyMetadata> assemblyMetadata);
    Task<Analysis> GenerateSummary(string logContent, DateTimeOffset utcNow, string title, int buildId, IEnumerable<WorkItemParam> workItems, IEnumerable<ChangeParam> changes, IEnumerable<AssemblyMetadata> assemblyMetadata);
    Task<Analysis> GenerateDetails(string logContent, DateTimeOffset utcNow, string title, int buildId, IEnumerable<WorkItemParam> workItems, IEnumerable<ChangeParam> changes, IEnumerable<AssemblyMetadata> assemblyMetadata);
    Task<Analysis> GeneratePerformanceAnalysis(string logContent, DateTimeOffset utcNow, string title, int buildId, IEnumerable<WorkItemParam> workItems, IEnumerable<ChangeParam> changes, IEnumerable<AssemblyMetadata> assemblyMetadata);

}