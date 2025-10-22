using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Backend.Api.Models.Email;

/// <summary>
/// Parameters for leaving a message via email
/// </summary>
public class LeaveMessageToolParams
{
    [Required]
    [Description("The sender's full name")]
    public required string fromName { get; set; }

    [Required]
    [Description("The sender's email address")]
    public required string fromEmail { get; set; }

    [Required]
    [Description("The subject of the message")]
    public required string subject { get; set; }

    [Required]
    [Description("The body of the message")]
    public required string body { get; set; }
}

