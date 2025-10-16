using Backend.Api.Models.OpenAI.Tools;
using Newtonsoft.Json;

namespace Backend.Api.Models.OpenAI.Request;

/// <summary>
/// Request model for OpenAI Chat Completions API
/// </summary>
public class CreateResponseRequest
{
    public required string input { get; set; }
    public string? previous_response_id { get; set; }
    public List<Tool>? tools { get; set; }

    [JsonProperty]
    public const string model = "gpt-4o";
    [JsonProperty]
    public const float temperature = 0.0001f;
    [JsonProperty]
    public const bool stream = true;
}

