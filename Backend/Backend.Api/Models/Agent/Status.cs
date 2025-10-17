using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Backend.Api.Models.Agent;

/// <summary>
/// Shared status enum with OpenAI
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum Status
{
    completed,
    failed,
    in_progress,
    cancelled,
    queued,
    incomplete,
}