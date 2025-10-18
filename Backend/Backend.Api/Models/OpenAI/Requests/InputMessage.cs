namespace Backend.Api.Models.OpenAI.Requests;

/// <summary>
/// A message input to the model with a role indicating instruction following hierarchy.
/// </summary>
public class InputMessage : InputItem
{
    /// <summary>
    /// The role of the message input. One of user, assistant, system, or developer.
    /// </summary>
    public MessageRole role { get; set; } = MessageRole.User;

    /// <summary>
    /// Text input to the model, used to generate a response.
    /// </summary>
    public string? content { get; set; }
}

