using Azure.Messaging.ServiceBus.Administration;
using Newtonsoft.Json;

namespace Diginsight.Tools.FeedMonitor;

public class EntityBase 
{
    public EntityBase()
    {
        this.Id = Guid.NewGuid().ToString();
        this.Type = this.GetType().Name;

        this.DateCreated = DateTimeOffset.UtcNow;
        this.DateModified = DateTimeOffset.UtcNow;
        this.Status = EntityStatus.Active;
    }

    [JsonProperty(PropertyName = "id", Required = Required.Always)]
    public string Id { get; set; }

    public EntityStatus Status { get; set; }

    public DateTimeOffset DateCreated { get; set; }

    public DateTimeOffset DateModified { get; set; }

    public string Type { get; set; }

    public string CreatedBy { get; set; }

    public string ModifiedBy { get; set; }
}
