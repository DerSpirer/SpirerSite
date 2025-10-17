using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Backend.Api.Models.OpenAI.Responses.Response;

namespace Backend.Api.Models.OpenAI.Converters;

/// <summary>
/// JSON converter for polymorphic ContentItem deserialization
/// </summary>
public class ContentItemConverter : JsonConverter<ContentItem>
{
    public override ContentItem? ReadJson(JsonReader reader, Type objectType, ContentItem? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        string? type = jsonObject["type"]?.Value<string>();

        ContentItem? item = type switch
        {
            "output_text" => jsonObject.ToObject<OutputText>(serializer),
            "refusal" => jsonObject.ToObject<Refusal>(serializer),
            "reasoning_text" => jsonObject.ToObject<ReasoningTextContent>(serializer),
            _ => null
        };

        return item;
    }

    public override void WriteJson(JsonWriter writer, ContentItem? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}

