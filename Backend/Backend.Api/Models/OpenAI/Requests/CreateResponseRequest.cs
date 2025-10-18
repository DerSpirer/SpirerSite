using Backend.Api.Models.OpenAI.Converters;
using Newtonsoft.Json;

namespace Backend.Api.Models.OpenAI.Requests;

/// <summary>
/// Request model for creating an OpenAI model response
/// </summary>
public class CreateResponseRequest
{
    /// <summary>
    /// Array of input items to the model, used to generate a response.
    /// Can include messages, function call outputs, and other input types.
    /// </summary>
    [JsonProperty(ItemConverterType = typeof(InputItemConverter))]
    public required List<InputItem> input { get; set; }

    /// <summary>
    /// Model ID used to generate the response, like gpt-4o or o3.
    /// </summary>
    public string model { get; set; } = "gpt-4o";

    /// <summary>
    /// Whether to allow the model to run tool calls in parallel. Defaults to true.
    /// </summary>
    public bool? parallel_tool_calls { get; set; } = false;

    /// <summary>
    /// The unique ID of the previous response to the model. Use this to create multi-turn conversations.
    /// </summary>
    public string? previous_response_id { get; set; }

    /// <summary>
    /// If set to true, the model response data will be streamed to the client as it is generated.
    /// </summary>
    public bool stream { get; set; } = true;

    /// <summary>
    /// An array of tools the model may call while generating a response.
    /// </summary>
    public List<Tool>? tools { get; set; } = new List<Tool>();

    /// <summary>
    /// An alternative to sampling with temperature, called nucleus sampling. 
    /// A value between 0 and 1. Defaults to 1.
    /// </summary>
    public double? top_p { get; set; }

    /// <summary>
    /// What sampling temperature to use, between 0 and 2. Defaults to 1.
    /// </summary>
    public double? temperature { get; set; } = 0.0001;
}

