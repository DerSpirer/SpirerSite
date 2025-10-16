namespace Backend.Api.Models.OpenAI.OutputItems;

public class FunctionCallOutputItem : OutputItem
{
    public string? name { get; set; }
    public string? arguments { get; set; }
    public string? call_id { get; set; }
}

