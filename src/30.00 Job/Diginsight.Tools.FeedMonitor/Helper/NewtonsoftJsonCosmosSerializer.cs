using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Diginsight.Tools.FeedMonitor;

public sealed class NewtonsoftJsonCosmosSerializer : CosmosSerializer
{
    public static readonly CosmosSerializer Instance = new NewtonsoftJsonCosmosSerializer();

    private static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);
    private static readonly JsonSerializerSettings SerializerSettings;

    static NewtonsoftJsonCosmosSerializer()
    {
        SerializerSettings = new JsonSerializerSettings()
        {
            //ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Include,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateParseHandling = DateParseHandling.DateTimeOffset,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            TypeNameHandling = TypeNameHandling.Auto,
            Converters =
            {
                new FeedChannelBaseConverter(), // Add the custom converter for FeedChannelBase
                new FeedItemBaseConverter(), // Add the custom converter for FeedItemBase
                new StringEnumConverter(),
            },
        };
    }

    private NewtonsoftJsonCosmosSerializer() { }

    public override T FromStream<T>(Stream stream)
    {
        using (stream)
        {
            if (typeof(Stream).IsAssignableFrom(typeof(T)))
            {
                Stream stream0 = stream;
                return Unsafe.As<Stream, T>(ref stream0);
            }

            using TextReader tr = new StreamReader(stream, DefaultEncoding, leaveOpen: true);
            using JsonReader jr = new JsonTextReader(tr);
            return GetSerializer().Deserialize<T>(jr);
        }
    }

    public override Stream ToStream<T>(T input)
    {
        MemoryStream memoryStream = new();

        if (input is Stream inputAsStream)
        {
            inputAsStream.CopyTo(memoryStream);
        }
        else
        {
            using TextWriter tw = new StreamWriter(memoryStream, DefaultEncoding, leaveOpen: true);
            // Use input.GetType() instead of typeof(T) to preserve polymorphic types
            GetSerializer().Serialize(tw, input, input?.GetType() ?? typeof(T));
        }

        memoryStream.Position = 0;
        return memoryStream;
    }

    private static JsonSerializer GetSerializer()
    {
        return JsonSerializer.Create(SerializerSettings);
    }
}
