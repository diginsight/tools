using DiginsightCopilotApi.Models;

namespace DiginsightCopilotApi.Abstractions;

public interface ISummaryService
{
    Task<Analysis> GenerateTitle(string logContent, IDictionary<string, object> placeholders);
    Task<Analysis> InferPlaceholders(string logContent, string title, IDictionary<string, object> placeholders);
    Task<Analysis> GenerateApplicationFlowInformation(string logContent, string title, IDictionary<string, object> placeholders);
    Task<Analysis> GenerateFullAnalysis(string logContent, string title, IDictionary<string, object> placeholders);
    Task<Analysis> GenerateSummary(string logContent, string title, IDictionary<string, object> placeholders);
    Task<Analysis> GenerateDetails(string logContent, string title, IDictionary<string, object> placeholders);
    Task<Analysis> GeneratePerformanceAnalysis(string logContent, string title, IDictionary<string, object> placeholders);

}