using Newtonsoft.Json.Schema;

namespace Backend.Api.Models.OpenAI.Tools;

/// <summary>
/// Represents a function definition for OpenAI function calling
/// </summary>
public class Function
{
    /// <summary>
    /// The name of the function to be called. Must be a-z, A-Z, 0-9, or contain underscores and dashes, with a maximum length of 64.
    /// </summary>
    public required string name { get; set; }

    /// <summary>
    /// A description of what the function does, used by the model to choose when and how to call the function.
    /// </summary>
    public string? description { get; set; }

    /// <summary>
    /// The parameters the function accepts, described as a JSON Schema object.
    /// </summary>
    public JSchema? parameters { get; set; }

    /// <summary>
    /// Whether to enable strict schema adherence when generating the function call.
    /// </summary>
    public bool? strict { get; set; }
}

