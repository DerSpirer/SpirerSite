namespace Backend.Api.Models.OpenAI.Objects.StreamingEvents;

/// <summary>
/// An event emitted during response streaming.
/// Types include: response.created, response.in_progress, response.completed, response.failed, response.incomplete
/// </summary>
public class ResponseEvent : StreamingEvent
{
    /// <summary>
    /// The response object associated with this event
    /// </summary>
    public required Response response { get; set; }
}

