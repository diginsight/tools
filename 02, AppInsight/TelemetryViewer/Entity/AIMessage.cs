using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TelemetryViewer
{
    public class AiMessage
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class Trace
    {
        public string message { get; set; }
        public int severityLevel { get; set; }
    }

    public class CustomDimensions
    {
        [JsonPropertyName("Entry.Category")]
        public string EntryCategory { get; set; }
        [JsonPropertyName("CodeSection.Type.Name")]
        public string CodeSectionTypeName { get; set; }
        [JsonPropertyName("Entry.ThreadID")]
        public string EntryThreadID { get; set; }
        [JsonPropertyName("CodeSection.OperationDept")]
        public string CodeSectionOperationDept { get; set; }
        [JsonPropertyName("Entry.SourceLevel")]
        public string EntrySourceLevel { get; set; }
        [JsonPropertyName("Entry.Source")]
        public string EntrySource { get; set; }
        [JsonPropertyName("Entry.Timestamp")]
        public string EntryTimestamp { get; set; }
        [JsonPropertyName("Entry.TraceEventType")]
        public string EntryTraceEventType { get; set; }
        [JsonPropertyName("CodeSection.NestingLevel")]
        public string CodeSectionNestingLevel { get; set; }
        [JsonPropertyName("Assembly.Name")]
        public string AssemblyName { get; set; }
        [JsonPropertyName("Entry.TraceSource")]
        public string EntryTraceSource { get; set; }
        [JsonPropertyName("Entry.Thread")]
        public string EntryThread { get; set; }
        [JsonPropertyName("Entry.ElapsedMilliseconds")]
        public string EntryElapsedMilliseconds { get; set; }
    }

    public class Operation
    {
        public string name { get; set; }
        public string id { get; set; }
        public string parentId { get; set; }
        public string syntheticSource { get; set; }
    }

    public class Session
    {
        public string id { get; set; }
    }

    public class User
    {
        public string id { get; set; }
        public string authenticatedId { get; set; }
        public string accountId { get; set; }
    }

    public class ClientApplication
    {
        public string version { get; set; }
    }

    public class Client
    {
        public string type { get; set; }
        public string model { get; set; }
        public string os { get; set; }
        public string ip { get; set; }
        public string city { get; set; }
        public string stateOrProvince { get; set; }
        public string countryOrRegion { get; set; }
        public string browser { get; set; }
    }

    public class Cloud
    {
        public string roleName { get; set; }
        public string roleInstance { get; set; }
    }

    public class AIApplication
    {
        public string appId { get; set; }
        public string appName { get; set; }
        public string iKey { get; set; }
        public string sdkVersion { get; set; }
    }

    public class AIEventValue
    {
        public string id { get; set; }
        public int count { get; set; }
        public string type { get; set; }
        public DateTime timestamp { get; set; }
        public Trace trace { get; set; }
        public CustomDimensions customDimensions { get; set; }
        public object customMeasurements { get; set; }
        public Operation operation { get; set; }
        public Session session { get; set; }
        public User user { get; set; }
        public ClientApplication application { get; set; }
        public Client client { get; set; }
        public Cloud cloud { get; set; }
        public AIApplication ai { get; set; }
    }
    public class RequestsDuration
    {
        public object avg { get; set; }
    }
    public class RequestsCount
    {
        public int sum { get; set; }
    }
    public class Segment
    {
        [JsonPropertyName("requests/count")]
        public RequestsCount RequestsCount { get; set; }
        [JsonPropertyName("client/countryOrRegion")]
        public string ClientCountryOrRegion { get; set; }
    }
    public class AIMetricsValue
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        [JsonPropertyName("requests/count")]
        public RequestsCount RequestsCount { get; set; }
        [JsonPropertyName("requests/duration")]
        public RequestsDuration RequestsDuration { get; set; }
        public List<Segment> segments { get; set; }
    }

    public class Innererror
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class Error
    {
        public string message { get; set; }
        public string code { get; set; }
        public Innererror innererror { get; set; }
    }

    public class AIEventsResult
    {
        [JsonPropertyName("@odata.context")]
        public string OdataContext { get; set; }
        [JsonPropertyName("@ai.messages")]
        public List<AiMessage> AiMessages { get; set; }
        public List<AIEventValue> value { get; set; }
        public Error error { get; set; }
    }
    public class AIMetricsResult
    {
        [JsonPropertyName("@odata.context")]
        public string OdataContext { get; set; }
        [JsonPropertyName("@ai.messages")]
        public List<AiMessage> AiMessages { get; set; }
        public AIMetricsValue value { get; set; }
        public Error error { get; set; }
    }


}
