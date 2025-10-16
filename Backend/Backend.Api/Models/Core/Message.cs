namespace Backend.Api.Models.Core;

/// <summary>
/// Represents a message in the conversation history
/// All fields are nullable because in stream we use only part of the message
/// </summary>
public class Message
{
    public string? id { get; set; }
    public Status? status { get; set; }
    public string? Role { get; set; }
    public string? Content { get; set; }
    public string? Refusal { get; set; }
    public string? Reasoning { get; set; }
    public string? ToolCallId { get; set; }
    public string? ToolName { get; set; }
    public string? ToolArguments { get; set; }

}