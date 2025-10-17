namespace Backend.Api.Models.OpenAI.Objects;

/// <summary>
/// Token usage details including input tokens, output tokens, and total tokens used
/// </summary>
public class Usage
{
    public int? input_tokens { get; set; }
    public InputTokensDetails? input_tokens_details { get; set; }
    public int? output_tokens { get; set; }
    public OutputTokensDetails? output_tokens_details { get; set; }
    public int? total_tokens { get; set; }
}

/// <summary>
/// A detailed breakdown of the input tokens
/// </summary>
public class InputTokensDetails
{
    public int? cached_tokens { get; set; }
}

/// <summary>
/// A detailed breakdown of the output tokens
/// </summary>
public class OutputTokensDetails
{
    public int? reasoning_tokens { get; set; }
}

