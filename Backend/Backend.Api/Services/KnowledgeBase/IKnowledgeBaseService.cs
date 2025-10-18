using Backend.Api.Models.VectorDatabase;

namespace Backend.Api.Services.KnowledgeBase;

/// <summary>
/// Service for managing knowledge base documents
/// </summary>
public interface IKnowledgeBaseService
{
    /// <summary>
    /// Refreshes a knowledge base document by deleting existing chunks and creating new ones
    /// </summary>
    /// <param name="documentName">The name of the document to refresh</param>
    /// <param name="content">The content of the document</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RefreshDocumentAsync(string documentName, string content, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Queries the knowledge base using semantic search
    /// </summary>
    /// <param name="query">The search query</param>
    /// <param name="topK">Number of results to return (default: 5)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of search results with content and scores</returns>
    Task<List<VectorSearchResult>> QueryAsync(string query, int topK = 5, CancellationToken cancellationToken = default);
}

