namespace Backend.Api.Models.OpenAI.Responses.StreamingEvents;

/// <summary>
/// An event emitted for reasoning text changes during response streaming.
/// Types include: response.reasoning_text.delta, response.reasoning_text.done
/// </summary>
public class ResponseReasoningTextEvent : StreamingEvent
{
    /// <summary>
    /// The index of the reasoning content part
    /// </summary>
    public required int content_index { get; set; }
    
    /// <summary>
    /// The text delta that was added to the reasoning content (present in response.reasoning_text.delta)
    /// </summary>
    public string? delta { get; set; }
    
    /// <summary>
    /// The ID of the item this reasoning text is associated with
    /// </summary>
    public required string item_id { get; set; }
    
    /// <summary>
    /// The index of the output item this reasoning text is associated with
    /// </summary>
    public required int output_index { get; set; }
    
    /// <summary>
    /// The full text of the completed reasoning content (present in response.reasoning_text.done)
    /// </summary>
    public string? text { get; set; }
}

