namespace Backend.Api.Models.OpenAI.Objects;

/// <summary>
/// A tool call to run a function
/// </summary>
public class FunctionToolCall : OutputItem
{
    public required string arguments { get; set; }
    public required string call_id { get; set; }
    public required string name { get; set; }
}

