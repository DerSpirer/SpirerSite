namespace Backend.Api.Models.OpenAI.Responses.StreamingEvents;

/// <summary>
/// An event emitted for output text changes during response streaming.
/// Types include: response.output_text.delta, response.output_text.done
/// </summary>
public class ResponseOutputTextEvent : StreamingEvent
{
    /// <summary>
    /// The index of the content part
    /// </summary>
    public required int content_index { get; set; }
    
    /// <summary>
    /// The text delta that was added (present in response.output_text.delta)
    /// </summary>
    public string? delta { get; set; }
    
    /// <summary>
    /// The ID of the output item
    /// </summary>
    public required string item_id { get; set; }
    
    /// <summary>
    /// The index of the output item
    /// </summary>
    public required int output_index { get; set; }
    
    /// <summary>
    /// The finalized text content (present in response.output_text.done)
    /// </summary>
    public string? text { get; set; }
}

