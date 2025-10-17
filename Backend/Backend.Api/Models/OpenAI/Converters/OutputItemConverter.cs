using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Backend.Api.Models.OpenAI.Responses.Response;

namespace Backend.Api.Models.OpenAI.Converters;

/// <summary>
/// JSON converter for polymorphic OutputItem deserialization
/// </summary>
public class OutputItemConverter : JsonConverter<OutputItem>
{
    public override OutputItem? ReadJson(JsonReader reader, Type objectType, OutputItem? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var type = jsonObject["type"]?.Value<string>();

        OutputItem? item = type switch
        {
            "message" => jsonObject.ToObject<OutputMessage>(),
            "function_call" => jsonObject.ToObject<FunctionToolCall>(),
            "reasoning" => jsonObject.ToObject<ReasoningOutput>(),
            _ => null
        };

        return item;
    }

    public override void WriteJson(JsonWriter writer, OutputItem? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}

