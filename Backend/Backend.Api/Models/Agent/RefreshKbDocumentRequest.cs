namespace Backend.Api.Models.Agent;

/// <summary>
/// Request model for refreshing a knowledge base document
/// </summary>
public class RefreshKbDocumentRequest
{
    /// <summary>
    /// The name of the document being refreshed
    /// </summary>
    public string DocumentName { get; set; } = string.Empty;

    /// <summary>
    /// The content of the document
    /// </summary>
    public string Content { get; set; } = string.Empty;
}

