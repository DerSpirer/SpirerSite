namespace Backend.Api.Models;

/// <summary>
/// Represents a search result from a vector database query
/// </summary>
public class VectorSearchResult
{
    /// <summary>
    /// The id of the matched document
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// The similarity score between the query and the document (higher is better)
    /// </summary>
    public required float Score { get; set; }

    /// <summary>
    /// The content of the matched document
    /// </summary>
    public required string Content { get; set; }
}

