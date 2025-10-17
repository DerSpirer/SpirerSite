using Newtonsoft.Json;

namespace Backend.Api.Models.OpenAI.Responses.Response;

/// <summary>
/// An output message from the model
/// </summary>
public class OutputMessage : OutputItem
{
    public List<ContentItem>? content { get; set; }
    public required string role { get; set; }
}

/// <summary>
/// Base class for content items in an output message
/// </summary>
[JsonConverter(typeof(ContentItemConverter))]
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
    public List<object>? annotations { get; set; }
}

/// <summary>
/// A refusal from the model
/// </summary>
public class Refusal : ContentItem
{
    public required string refusal { get; set; }
}

