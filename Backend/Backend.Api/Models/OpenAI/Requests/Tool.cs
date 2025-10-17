namespace Backend.Api.Models.OpenAI.Requests;

/// <summary>
/// Base class for tools the model may call while generating a response.
/// </summary>
public abstract class Tool
{
    /// <summary>
    /// The type of the tool (e.g., "function", "file_search", "web_search", etc.)
    /// </summary>
    public abstract string type { get; }
}

