using Backend.Api.Models.OpenAI.Responses.Response;
using Newtonsoft.Json;

namespace Backend.Api.Models.OpenAI.Responses.StreamingEvents;

/// <summary>
/// An event emitted for output item changes during response streaming.
/// Types include: response.output_item.added, response.output_item.done
/// </summary>
public class ResponseOutputItemEvent : StreamingEvent
{
    /// <summary>
    /// The output item associated with this event
    /// </summary>
    [JsonConverter(typeof(OutputItemConverter))]
    public required OutputItem item { get; set; }
    
    /// <summary>
    /// The index of the output item
    /// </summary>
    public required int output_index { get; set; }
}

