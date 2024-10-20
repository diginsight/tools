using DiginsightCopilotApi.Models;

namespace DiginsightCopilotApi.Abstractions;

public interface ISummaryService
{
    Task<Analysis> GenerateFullSummary(string logContent, int buildId, IEnumerable<WorkItemParam> workItems, IEnumerable<ChangeParam> changes, IEnumerable<AssemblyMetadata> assemblyMetadata);
    Task<Analysis> GenerateTitle(string logContent, int buildId, IEnumerable<WorkItemParam> workItems, IEnumerable<ChangeParam> changes, IEnumerable<AssemblyMetadata> assemblyMetadata);
    Task<Analysis> InferPlaceholders(string logContent, string title, int buildId, IEnumerable<WorkItemParam> workItems, IEnumerable<ChangeParam> changes, IEnumerable<AssemblyMetadata> assemblyMetadata);
    
}