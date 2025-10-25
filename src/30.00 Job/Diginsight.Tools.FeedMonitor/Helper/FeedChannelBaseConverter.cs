using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Diginsight.Tools.FeedMonitor;

public class FeedChannelBaseConverter : JsonConverter
{
    public override bool CanWrite => false; // Use default serialization

    public override bool CanConvert(Type objectType)
    {
        // Only handle FeedChannelBase and its derived types
 return typeof(FeedChannelBase).IsAssignableFrom(objectType);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        // Handle null values
     if (reader.TokenType == JsonToken.Null)
        {
 return null;
        }

        // Load the JSON object
        JObject jsonObject = JObject.Load(reader);

        // Get the Type discriminator property
        string? typeName = jsonObject["Type"]?.Value<string>();

    if (string.IsNullOrEmpty(typeName))
        {
       throw new JsonSerializationException("Missing 'Type' discriminator property in FeedChannelBase JSON.");
        }

        // Determine the concrete type based on the Type property
        FeedChannelBase? target = typeName switch
        {
nameof(RSSFeedChannel) => new RSSFeedChannel(),
       nameof(AtomFeedChannel) => new AtomFeedChannel(),
        _ => throw new JsonSerializationException($"Unknown feed channel type: {typeName}")
      };

        // Populate the object using the serializer
        using (JsonReader objectReader = jsonObject.CreateReader())
  {
   serializer.Populate(objectReader, target);
      }

     return target;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException("Use default serialization.");
    }
}
