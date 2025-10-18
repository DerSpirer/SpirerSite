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
}

