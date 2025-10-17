using Newtonsoft.Json;

namespace Backend.Api.Models.OpenAI.Responses.StreamingEvents;

/// <summary>
/// Base class for all streaming events from OpenAI API
/// </summary>
public abstract class StreamingEvent
{
    /// <summary>
    /// The sequence number of this event
    /// </summary>
    public required int sequence_number { get; set; }
    
    /// <summary>
    /// The type of the event
    /// </summary>
    public required string type { get; set; }
}

