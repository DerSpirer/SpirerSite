namespace Backend.Api.Models.OpenAI.OutputItems;

public class ReasoningOutputItem : OutputItem
{
    public List<ReasoningContentItem>? content { get; set; }

    public class ReasoningContentItem
{
    public string? text { get; set; }
}
}

