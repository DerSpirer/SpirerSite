using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Backend.Api.Models.OpenAI.Objects.StreamingEvents;

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
            "response.created" => jsonObject.ToObject<ResponseEvent>(),
            "response.in_progress" => jsonObject.ToObject<ResponseEvent>(),
            "response.completed" => jsonObject.ToObject<ResponseEvent>(),
            "response.failed" => jsonObject.ToObject<ResponseEvent>(),
            "response.incomplete" => jsonObject.ToObject<ResponseEvent>(),
            
            "response.output_item.added" => jsonObject.ToObject<ResponseOutputItemEvent>(),
            "response.output_item.done" => jsonObject.ToObject<ResponseOutputItemEvent>(),
            
            "response.content_part.added" => jsonObject.ToObject<ResponseContentPartEvent>(),
            "response.content_part.done" => jsonObject.ToObject<ResponseContentPartEvent>(),
            
            "response.output_text.delta" => jsonObject.ToObject<ResponseOutputTextEvent>(),
            "response.output_text.done" => jsonObject.ToObject<ResponseOutputTextEvent>(),
            
            "response.refusal.delta" => jsonObject.ToObject<ResponseRefusalEvent>(),
            "response.refusal.done" => jsonObject.ToObject<ResponseRefusalEvent>(),
            
            "response.function_call_arguments.delta" => jsonObject.ToObject<ResponseFunctionCallArgumentsEvent>(),
            "response.function_call_arguments.done" => jsonObject.ToObject<ResponseFunctionCallArgumentsEvent>(),
            
            "response.reasoning_text.delta" => jsonObject.ToObject<ResponseReasoningTextEvent>(),
            "response.reasoning_text.done" => jsonObject.ToObject<ResponseReasoningTextEvent>(),
            
            _ => null
        };

        return streamingEvent;
    }

    public override void WriteJson(JsonWriter writer, StreamingEvent? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}

