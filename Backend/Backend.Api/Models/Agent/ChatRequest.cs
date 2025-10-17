using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Backend.Api.Models.Agent;

/// <summary>
/// Request model for chat endpoint
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// The input string to send to the agent
    /// </summary>
    [Required]
    [Description("The input string to send to the agent")]
    public required string Input { get; set; }

    /// <summary>
    /// Optional previous response ID for context
    /// </summary>
    [Description("Optional previous response ID for context")]
    public string? PreviousResponseId { get; set; }
}

