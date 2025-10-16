using Backend.Api.Models.OpenAI.Streaming;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Backend.Api.Models.OpenAI.Converters;

/// <summary>
/// JSON converter for polymorphic StreamEvent deserialization
/// </summary>
public class StreamEventConverter : JsonConverter<StreamEvent>
{
    public override StreamEvent? ReadJson(
        JsonReader reader,
        Type objectType,
        StreamEvent? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var jsonObject = JObject.Load(reader);
        var type = jsonObject["type"]?.ToString();

        if (string.IsNullOrEmpty(type))
            return null;

        StreamEvent? streamEvent = type switch
        {
            // Response-level events
            "response.created" or 
            "response.in_progress" or 
            "response.completed" or 
            "response.failed" or 
            "response.cancelled" => jsonObject.ToObject<ResponseEvent>(serializer),
            
            // Output item events
            "response.output_item.added" or 
            "response.output_item.done" => jsonObject.ToObject<OutputItemEvent>(serializer),
            
            // Content part events
            "response.content_part.added" or 
            "response.content_part.done" => jsonObject.ToObject<ContentPartEvent>(serializer),
            
            // Text delta event
            "response.output_text.delta" => jsonObject.ToObject<TextDeltaEvent>(serializer),
            
            // Text done event
            "response.output_text.done" => jsonObject.ToObject<TextDoneEvent>(serializer),
            
            _ => null
        };

        return streamEvent;
    }

    public override void WriteJson(JsonWriter writer, StreamEvent? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var jsonObject = JObject.FromObject(value, serializer);
        jsonObject.WriteTo(writer);
    }

    public override bool CanRead => true;
    public override bool CanWrite => true;
}

