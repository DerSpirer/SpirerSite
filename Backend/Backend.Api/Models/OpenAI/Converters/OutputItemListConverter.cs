using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Backend.Api.Models.OpenAI.Responses.Response;

namespace Backend.Api.Models.OpenAI.Converters;

/// <summary>
/// JSON converter for List of polymorphic OutputItem deserialization
/// </summary>
public class OutputItemListConverter : JsonConverter<List<OutputItem>>
{
    private static readonly OutputItemConverter _itemConverter = new OutputItemConverter();
    
    public override List<OutputItem>? ReadJson(JsonReader reader, Type objectType, List<OutputItem>? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        JArray jsonArray = JArray.Load(reader);
        List<OutputItem> items = new List<OutputItem>();

        foreach (JToken token in jsonArray)
        {
            using (JsonReader tokenReader = token.CreateReader())
            {
                OutputItem? item = _itemConverter.ReadJson(tokenReader, typeof(OutputItem), null, false, serializer);
                if (item != null)
                {
                    items.Add(item);
                }
            }
        }

        return items;
    }

    public override void WriteJson(JsonWriter writer, List<OutputItem>? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartArray();
        foreach (OutputItem item in value)
        {
            serializer.Serialize(writer, item);
        }
        writer.WriteEndArray();
    }
}

