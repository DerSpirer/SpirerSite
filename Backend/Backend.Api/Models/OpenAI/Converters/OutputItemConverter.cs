using Backend.Api.Models.OpenAI.OutputItems;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Backend.Api.Models.OpenAI.Converters;

/// <summary>
/// JSON converter for polymorphic OutputItem list deserialization
/// </summary>
public class OutputItemListConverter : JsonConverter<List<OutputItem>>
{
    public override List<OutputItem>? ReadJson(
        JsonReader reader,
        Type objectType,
        List<OutputItem>? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var array = JArray.Load(reader);
        var result = new List<OutputItem>();

        foreach (var item in array)
        {
            var type = item["type"]?.ToString()?.ToLowerInvariant();

            OutputItem? outputItem = type switch
            {
                "message" => item.ToObject<MessageOutputItem>(serializer),
                "function_call" => item.ToObject<FunctionCallOutputItem>(serializer),
                "reasoning" => item.ToObject<ReasoningOutputItem>(serializer),
                _ => null
            };

            if (outputItem != null)
            {
                result.Add(outputItem);
            }
        }

        return result;
    }

    public override void WriteJson(JsonWriter writer, List<OutputItem>? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartArray();

        foreach (var item in value)
        {
            var jsonObject = JObject.FromObject(item, serializer);

            // Add type discriminator based on the concrete type
            string typeValue = item switch
            {
                MessageOutputItem => "message",
                FunctionCallOutputItem => "function_call",
                ReasoningOutputItem => "reasoning",
                _ => "unknown"
            };

            jsonObject["type"] = typeValue;
            jsonObject.WriteTo(writer);
        }

        writer.WriteEndArray();
    }
}

/// <summary>
/// JSON converter for polymorphic OutputItem deserialization
/// </summary>
public class OutputItemConverter : JsonConverter<OutputItem>
{
    public override OutputItem? ReadJson(
        JsonReader reader, 
        Type objectType, 
        OutputItem? existingValue, 
        bool hasExistingValue, 
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return null;

        var jsonObject = JObject.Load(reader);
        var type = jsonObject["type"]?.ToString()?.ToLowerInvariant();

        OutputItem? item = type switch
        {
            "message" => new MessageOutputItem(),
            "function_call" => new FunctionCallOutputItem(),
            "reasoning" => new ReasoningOutputItem(),
            _ => null
        };

        if (item != null)
        {
            serializer.Populate(jsonObject.CreateReader(), item);
        }

        return item;
    }

    public override void WriteJson(JsonWriter writer, OutputItem? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var jsonObject = JObject.FromObject(value, serializer);
        
        // Add type discriminator based on the concrete type
        string typeValue = value switch
        {
            MessageOutputItem => "message",
            FunctionCallOutputItem => "function_call",
            ReasoningOutputItem => "reasoning",
            _ => "unknown"
        };

        jsonObject["type"] = typeValue;
        jsonObject.WriteTo(writer);
    }

    public override bool CanRead => true;
    public override bool CanWrite => true;
}

