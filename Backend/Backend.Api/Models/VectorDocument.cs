namespace Backend.Api.Models;

/// <summary>
/// Represents a document with vector embeddings for storage and retrieval in a vector database
/// </summary>
public class VectorDocument
{
    /// <summary>
    /// Unique identifier for the document
    /// </summary>
    public required string Id { get; set; }
    
    /// <summary>
    /// The vector embedding representation of the document
    /// </summary>
    public required float[] Vector { get; set; }
    
    /// <summary>
    /// The original text content of the document
    /// </summary>
    public required string Content { get; set; }
}

