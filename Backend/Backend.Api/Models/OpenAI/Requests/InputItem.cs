using Backend.Api.Models.Agent;

namespace Backend.Api.Models.OpenAI.Requests;

/// <summary>
/// Base class for input items to the model
/// </summary>
public abstract class InputItem
{
    /// <summary>
    /// The type of the input item (e.g., "message", "function_call_output")
    /// </summary>
    public required string type { get; set; }
    
    /// <summary>
    /// The unique ID of the input item. Optional, populated when items are returned via API.
    /// </summary>
    public string? id { get; set; }
    
    /// <summary>
    /// The status of the item. One of in_progress, completed, or incomplete.
    /// </summary>
    public Status? status { get; set; }
}

