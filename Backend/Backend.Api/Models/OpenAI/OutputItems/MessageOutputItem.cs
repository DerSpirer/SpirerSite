namespace Backend.Api.Models.OpenAI.OutputItems;

public class MessageOutputItem : OutputItem
{
    public string? role { get; set; }
    public List<ContentItem>? content { get; set; }
}

