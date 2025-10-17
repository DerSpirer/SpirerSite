using Backend.Api.Models.Agent;
using Backend.Api.Models.OpenAI.Requests;
using Backend.Api.Models.OpenAI.Converters;
using Newtonsoft.Json;

namespace Backend.Api.Models.OpenAI.Responses.Response;

/// <summary>
/// An output message from the model
/// </summary>
public class OutputMessage : OutputItem
{
    [JsonConverter(typeof(ContentItemListConverter))]
    public List<ContentItem>? content { get; set; }
    public MessageRole? role { get; set; }
}

/// <summary>
/// Base class for content items in an output message
/// </summary>
public abstract class ContentItem
{
    public required string type { get; set; }
}

/// <summary>
/// A text output from the model
/// </summary>
public class OutputText : ContentItem
{
    public required string text { get; set; }
}

/// <summary>
/// A refusal from the model
/// </summary>
public class Refusal : ContentItem
{
    public required string refusal { get; set; }
}

/// <summary>
/// Reasoning text content from the model (used in streaming)
/// </summary>
public class ReasoningTextContent : ContentItem
{
    public required string text { get; set; }
}

