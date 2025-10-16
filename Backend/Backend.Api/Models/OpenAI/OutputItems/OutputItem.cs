using Backend.Api.Models.Core;

namespace Backend.Api.Models.OpenAI.OutputItems;

public abstract class OutputItem
{
    public string? id { get; set; } 
    public Status? status { get; set; }
}

