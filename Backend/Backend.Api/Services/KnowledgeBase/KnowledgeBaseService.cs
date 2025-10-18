using Backend.Api.Models.VectorDatabase;
using Backend.Api.Services.Embedding;
using Backend.Api.Services.VectorDatabase;
using Pinecone;

namespace Backend.Api.Services.KnowledgeBase;

/// <summary>
/// Service for managing knowledge base documents
/// </summary>
public class KnowledgeBaseService : IKnowledgeBaseService
{
    private const string NAMESPACE_ID = "agent-kb";
    private const int CHUNK_SIZE = 500; // Number of words per chunk
    private const int OVERLAP_SIZE = 50; // Number of words to overlap before and after

    private readonly IVectorDatabaseService _vectorDatabaseService;
    private readonly IEmbeddingService _embeddingService;
    private readonly ILogger<KnowledgeBaseService> _logger;

    public KnowledgeBaseService(
        IVectorDatabaseService vectorDatabaseService,
        IEmbeddingService embeddingService,
        ILogger<KnowledgeBaseService> logger)
    {
        _vectorDatabaseService = vectorDatabaseService;
        _embeddingService = embeddingService;
        _logger = logger;
    }

    public async Task RefreshDocumentAsync(string documentName, string content, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Refreshing document: {documentName}");

        // Step 1: Delete existing vectors for this document
        _logger.LogInformation($"Deleting existing vectors for document: {documentName}");
        Metadata filter = new(new Dictionary<string, MetadataValue?>
        {
            { "documentName", new Metadata(new Dictionary<string, MetadataValue?> { { "$eq", documentName } }) }
        });
        await _vectorDatabaseService.DeleteByMetadata(NAMESPACE_ID, filter);

        // Step 2: Chunk the content with overlap
        _logger.LogInformation($"Chunking document: {documentName}");
        List<DocumentChunk> chunks = ChunkDocument(documentName, content);
        _logger.LogInformation($"Created {chunks.Count} chunks for document: {documentName}");

        // Step 3: Generate embeddings for each chunk (using embeddingText, not the full content with overlap)
        _logger.LogInformation($"Generating embeddings for {chunks.Count} chunks");
        IList<float[]> embeddings = await _embeddingService.GenerateEmbeddingsAsync(
            chunks.Select(c => c.EmbeddingText), 
            cancellationToken);

        // Step 4: Create VectorDocuments
        List<VectorDocument> vectorDocuments = chunks.Select((chunk, index) => new VectorDocument
        {
            Id = chunk.Id,
            Vector = embeddings[index],
            Content = chunk.Content, // Full content with overlap
            DocumentName = chunk.DocumentName,
            EmbeddingText = chunk.EmbeddingText // Core text without overlap
        }).ToList();

        // Step 5: Upsert to vector database
        _logger.LogInformation($"Upserting {vectorDocuments.Count} vectors for document: {documentName}");
        await _vectorDatabaseService.Upsert(NAMESPACE_ID, vectorDocuments);

        _logger.LogInformation($"Successfully refreshed document: {documentName}");
    }

    public async Task<List<VectorSearchResult>> QueryAsync(string query, int topK = 5, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Querying knowledge base with query: {query}, topK: {topK}");
        
        try
        {
            List<VectorSearchResult> results = await _vectorDatabaseService.Search(NAMESPACE_ID, query, topK);
            
            _logger.LogInformation($"Found {results.Count} results for query: {query}");
            
            return results;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Error querying knowledge base with query: {query}");
            throw;
        }
    }

    private List<DocumentChunk> ChunkDocument(string documentName, string content)
    {
        List<DocumentChunk> chunks = new();
        
        // Split content into words
        string[] words = content.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        
        int index = 0;
        int chunkNumber = 0;

        while (index < words.Length)
        {
            // Calculate the range for the core chunk (without overlap)
            int coreStart = index;
            int coreEnd = Math.Min(index + CHUNK_SIZE, words.Length);
            
            // Calculate the range for the full chunk (with overlap)
            int fullStart = Math.Max(0, coreStart - OVERLAP_SIZE);
            int fullEnd = Math.Min(words.Length, coreEnd + OVERLAP_SIZE);

            // Get the core text (for embedding)
            string embeddingText = string.Join(" ", words[coreStart..coreEnd]);
            
            // Get the full text (with overlap, for context)
            string fullContent = string.Join(" ", words[fullStart..fullEnd]);

            chunks.Add(new DocumentChunk
            {
                Id = $"{documentName}_{chunkNumber}",
                DocumentName = documentName,
                EmbeddingText = embeddingText,
                Content = fullContent
            });

            // Move to the next chunk
            index += CHUNK_SIZE;
            chunkNumber++;
        }

        return chunks;
    }

    /// <summary>
    /// Internal model for document chunks
    /// </summary>
    private class DocumentChunk
    {
        public required string Id { get; set; }
        public required string DocumentName { get; set; }
        public required string EmbeddingText { get; set; }
        public required string Content { get; set; }
    }
}

