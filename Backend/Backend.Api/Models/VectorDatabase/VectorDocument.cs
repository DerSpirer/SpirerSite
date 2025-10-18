namespace Backend.Api.Models.VectorDatabase;

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
    /// The original text content of the document (may include overlap text for context)
    /// </summary>
    public required string Content { get; set; }
    
    /// <summary>
    /// The name of the source document this chunk belongs to
    /// </summary>
    public required string DocumentName { get; set; }
    
    /// <summary>
    /// The core text that was used to generate the embedding (without overlap)
    /// </summary>
    public required string EmbeddingText { get; set; }
}

