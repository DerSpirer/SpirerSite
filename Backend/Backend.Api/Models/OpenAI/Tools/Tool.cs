namespace Backend.Api.Models.OpenAI.Tools;

/// <summary>
/// Represents a tool that the model may call. Currently, only functions are supported as a tool.
/// </summary>
public class Tool
{
    /// <summary>
    /// The type of the tool. Currently, only 'function' is supported.
    /// </summary>
    public string type { get; set; } = "function";

    /// <summary>
    /// The function definition
    /// </summary>
    public required Function function { get; set; }
}

