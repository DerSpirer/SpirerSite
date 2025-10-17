namespace Backend.Api.Models.OpenAI.Objects.StreamingEvents;

/// <summary>
/// An event emitted for refusal text changes during response streaming.
/// Types include: response.refusal.delta, response.refusal.done
/// </summary>
public class ResponseRefusalEvent : StreamingEvent
{
    /// <summary>
    /// The index of the content part
    /// </summary>
    public required int content_index { get; set; }
    
    /// <summary>
    /// The refusal text delta that is added (present in response.refusal.delta)
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
    /// The finalized refusal text (present in response.refusal.done)
    /// </summary>
    public string? refusal { get; set; }
}

