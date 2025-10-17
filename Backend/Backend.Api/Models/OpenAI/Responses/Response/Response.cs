namespace Backend.Api.Models.OpenAI.Responses.Response;

/// <summary>
/// Response object from OpenAI API
/// </summary>
public class Response
{
    public required string id { get; set; }
    public ErrorDetails? error { get; set; }
    public required long created_at { get; set; }
    public IncompleteDetails? incomplete_details { get; set; }
    public required string model { get; set; }
    public required string @object { get; set; }
    public List<OutputItem>? output { get; set; }
    public required string status { get; set; }
    public Usage? usage { get; set; }
}

/// <summary>
/// Error details when the model fails to generate a response
/// </summary>
public class ErrorDetails
{
    public required string code { get; set; }
    public required string message { get; set; }
}

/// <summary>
/// Details about why the response is incomplete
/// </summary>
public class IncompleteDetails
{
    public required string reason { get; set; }
}

