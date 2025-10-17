using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Backend.Api.Models.OpenAI.Responses.StreamingEvents;

namespace Backend.Api.Models.OpenAI.Converters;

/// <summary>
/// JSON converter for polymorphic StreamingEvent deserialization
/// </summary>
public class StreamingEventConverter : JsonConverter<StreamingEvent>
{
    public override StreamingEvent? ReadJson(JsonReader reader, Type objectType, StreamingEvent? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jsonObject = JObject.Load(reader);
        string? type = jsonObject["type"]?.Value<string>();

        StreamingEvent? streamingEvent = type switch
        {
            "response.created" => jsonObject.ToObject<ResponseEvent>(serializer),
            "response.in_progress" => jsonObject.ToObject<ResponseEvent>(serializer),
            "response.completed" => jsonObject.ToObject<ResponseEvent>(serializer),
            "response.failed" => jsonObject.ToObject<ResponseEvent>(serializer),
            "response.incomplete" => jsonObject.ToObject<ResponseEvent>(serializer),
            
            "response.output_item.added" => jsonObject.ToObject<ResponseOutputItemEvent>(serializer),
            "response.output_item.done" => jsonObject.ToObject<ResponseOutputItemEvent>(serializer),
            
            "response.content_part.added" => jsonObject.ToObject<ResponseContentPartEvent>(serializer),
            "response.content_part.done" => jsonObject.ToObject<ResponseContentPartEvent>(serializer),
            
            "response.output_text.delta" => jsonObject.ToObject<ResponseOutputTextEvent>(serializer),
            "response.output_text.done" => jsonObject.ToObject<ResponseOutputTextEvent>(serializer),
            
            "response.refusal.delta" => jsonObject.ToObject<ResponseRefusalEvent>(serializer),
            "response.refusal.done" => jsonObject.ToObject<ResponseRefusalEvent>(serializer),
            
            "response.function_call_arguments.delta" => jsonObject.ToObject<ResponseFunctionCallArgumentsEvent>(serializer),
            "response.function_call_arguments.done" => jsonObject.ToObject<ResponseFunctionCallArgumentsEvent>(serializer),
            
            "response.reasoning_text.delta" => jsonObject.ToObject<ResponseReasoningTextEvent>(serializer),
            "response.reasoning_text.done" => jsonObject.ToObject<ResponseReasoningTextEvent>(serializer),
            
            _ => null
        };

        return streamingEvent;
    }

    public override void WriteJson(JsonWriter writer, StreamingEvent? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}

