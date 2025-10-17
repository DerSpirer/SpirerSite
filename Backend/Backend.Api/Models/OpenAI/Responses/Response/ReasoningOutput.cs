namespace Backend.Api.Models.OpenAI.Responses.Response;

/// <summary>
/// A description of the chain of thought used by a reasoning model while generating a response
/// </summary>
public class ReasoningOutput : OutputItem
{
    public List<object>? summary { get; set; }
    public List<ReasoningText>? content { get; set; }
    public string? encrypted_content { get; set; }
}

/// <summary>
/// Reasoning text content from the model
/// </summary>
public class ReasoningText
{
    public required string text { get; set; }
    public required string type { get; set; }
}

