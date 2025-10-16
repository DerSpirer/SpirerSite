using Backend.Api.Models.OpenAI.Converters;
using Backend.Api.Models.OpenAI.OutputItems;
using Backend.Api.Models.OpenAI.Responses;
using Newtonsoft.Json;

namespace Backend.Api.Models.OpenAI.Streaming;

/// <summary>
/// Base class for OpenAI Server-Sent Events
/// </summary>
[JsonConverter(typeof(StreamEventConverter))]
public abstract class StreamEvent
{
    public string type { get; set; } = string.Empty;
    
    public int sequence_number { get; set; }
}

/// <summary>
/// Response-level events (created, in_progress, completed, failed, cancelled)
/// </summary>
public class ResponseEvent : StreamEvent
{
    public Response response { get; set; } = new();
}

/// <summary>
/// Output item events (added, done)
/// </summary>
public class OutputItemEvent : StreamEvent
{
    public int output_index { get; set; }
    
    public OutputItem? item { get; set; }
}

/// <summary>
/// Base class for content-related events with common properties
/// </summary>
public abstract class ContentEventBase : StreamEvent
{
    public string item_id { get; set; } = string.Empty;
    
    public int output_index { get; set; }
    
    public int content_index { get; set; }
}

/// <summary>
/// Content part events (added, done)
/// </summary>
public class ContentPartEvent : ContentEventBase
{
    public ContentItem? part { get; set; }
}

/// <summary>
/// Text delta event (streaming text chunks)
/// </summary>
public class TextDeltaEvent : ContentEventBase
{
    public string delta { get; set; } = string.Empty;
}

/// <summary>
/// Text done event (complete text)
/// </summary>
public class TextDoneEvent : ContentEventBase
{
    public string text { get; set; } = string.Empty;
}


