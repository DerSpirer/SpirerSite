using Newtonsoft.Json;
using Backend.Api.Models.OpenAI.Converters;

namespace Backend.Api.Models.OpenAI.Objects.StreamingEvents;

/// <summary>
/// An event emitted for content part changes during response streaming.
/// Types include: response.content_part.added, response.content_part.done
/// </summary>
public class ResponseContentPartEvent : StreamingEvent
{
    /// <summary>
    /// The index of the content part
    /// </summary>
    public required int content_index { get; set; }
    
    /// <summary>
    /// The ID of the output item that the content part belongs to
    /// </summary>
    public required string item_id { get; set; }
    
    /// <summary>
    /// The index of the output item that the content part belongs to
    /// </summary>
    public required int output_index { get; set; }
    
    /// <summary>
    /// The content part (can be OutputText, Refusal, or ReasoningTextContent)
    /// </summary>
    [JsonConverter(typeof(ContentItemConverter))]
    public required ContentItem part { get; set; }
}

