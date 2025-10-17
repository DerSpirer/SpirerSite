using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Backend.Api.Models.OpenAI.Responses.Response;

namespace Backend.Api.Models.OpenAI.Converters;

/// <summary>
/// JSON converter for List of polymorphic ContentItem deserialization
/// </summary>
public class ContentItemListConverter : JsonConverter<List<ContentItem>>
{
    private static readonly ContentItemConverter _itemConverter = new ContentItemConverter();
    
    public override List<ContentItem>? ReadJson(JsonReader reader, Type objectType, List<ContentItem>? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        JArray jsonArray = JArray.Load(reader);
        List<ContentItem> items = new List<ContentItem>();

        foreach (JToken token in jsonArray)
        {
            using (JsonReader tokenReader = token.CreateReader())
            {
                ContentItem? item = _itemConverter.ReadJson(tokenReader, typeof(ContentItem), null, false, serializer);
                if (item != null)
                {
                    items.Add(item);
                }
            }
        }

        return items;
    }

    public override void WriteJson(JsonWriter writer, List<ContentItem>? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartArray();
        foreach (ContentItem item in value)
        {
            serializer.Serialize(writer, item);
        }
        writer.WriteEndArray();
    }
}

