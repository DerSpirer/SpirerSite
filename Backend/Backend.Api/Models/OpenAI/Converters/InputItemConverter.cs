using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Backend.Api.Models.OpenAI.Requests;

namespace Backend.Api.Models.OpenAI.Converters;

/// <summary>
/// JSON converter for polymorphic InputItem deserialization
/// </summary>
public class InputItemConverter : JsonConverter<InputItem>
{
    public override InputItem? ReadJson(JsonReader reader, Type objectType, InputItem? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var type = jsonObject["type"]?.Value<string>();

        InputItem? item = type switch
        {
            "message" => jsonObject.ToObject<InputMessage>(),
            "function_call_output" => jsonObject.ToObject<FunctionToolCallOutput>(),
            _ => null
        };

        return item;
    }

    public override void WriteJson(JsonWriter writer, InputItem? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}

