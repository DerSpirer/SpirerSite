using Newtonsoft.Json.Schema;

namespace Backend.Api.Models.OpenAI.Requests;

/// <summary>
/// Defines a function in your own code the model can choose to call.
/// Learn more about function calling: https://platform.openai.com/docs/guides/function-calling
/// </summary>
public class FunctionTool : Tool
{
    /// <summary>
    /// The type of the function tool. Always "function".
    /// </summary>
    public override string type => "function";

    /// <summary>
    /// The name of the function to call.
    /// </summary>
    public required string name { get; set; }

    /// <summary>
    /// A JSON schema object describing the parameters of the function.
    /// </summary>
    public required JSchema parameters { get; set; }

    /// <summary>
    /// Whether to enforce strict parameter validation. Default true.
    /// </summary>
    public bool strict { get; set; } = true;

    /// <summary>
    /// A description of the function. Used by the model to determine whether or not to call the function.
    /// </summary>
    public string? description { get; set; }
}

