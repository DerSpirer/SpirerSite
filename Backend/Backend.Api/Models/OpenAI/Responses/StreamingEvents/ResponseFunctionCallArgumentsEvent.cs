namespace Backend.Api.Models.OpenAI.Responses.StreamingEvents;

/// <summary>
/// An event emitted for function call arguments changes during response streaming.
/// Types include: response.function_call_arguments.delta, response.function_call_arguments.done
/// </summary>
public class ResponseFunctionCallArgumentsEvent : StreamingEvent
{
    /// <summary>
    /// The finalized function call arguments (present in response.function_call_arguments.done)
    /// </summary>
    public string? arguments { get; set; }
    
    /// <summary>
    /// The function call arguments delta that is added (present in response.function_call_arguments.delta)
    /// </summary>
    public string? delta { get; set; }
    
    /// <summary>
    /// The ID of the output item
    /// </summary>
    public required string item_id { get; set; }
    
    /// <summary>
    /// The name of the function that was called (present in response.function_call_arguments.done)
    /// </summary>
    public string? name { get; set; }
    
    /// <summary>
    /// The index of the output item
    /// </summary>
    public required int output_index { get; set; }
}

