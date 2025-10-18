using Backend.Api.Models.VectorDatabase;
using Backend.Api.Services.Embedding;
using Pinecone;

namespace Backend.Api.Services.VectorDatabase;

public class PineconeVectorDatabaseService : IVectorDatabaseService
{
    private readonly IEmbeddingService _embeddingService;
    private readonly PineconeClient _pineconeClient;
    private readonly IndexClient _index;

    public PineconeVectorDatabaseService(IConfiguration configuration, IEmbeddingService embeddingService)
    {
        string apiKey = configuration["PineconeApiKey"] ?? throw new InvalidOperationException("PineconeApiKey is not set");
        _pineconeClient = new PineconeClient(apiKey);
        string indexName = configuration["PineconeIndexName"] ?? throw new InvalidOperationException("PineconeIndexName is not set");
        string indexHost = configuration["PineconeIndexHost"] ?? throw new InvalidOperationException("PineconeIndexHost is not set");
        _index = _pineconeClient.Index(indexName, indexHost);
        _embeddingService = embeddingService;
    }

    public async Task<List<VectorSearchResult>> Search(string namespaceId, string queryText, int topK = 10)
    {
        try
        {
            float[] queryVector = await _embeddingService.GenerateEmbeddingAsync(queryText);
            SearchRecordsRequest request = new()
            {
                Query = new()
                {
                    TopK = topK,
                    Vector = new()
                    {
                        Values = queryVector,
                    },
                },
            };
            SearchRecordsResponse result = await _index.SearchRecordsAsync(namespaceId, request);
            List<Hit> hits = result.Result.Hits.ToList();
            List<VectorSearchResult> results = hits.Select(h => new VectorSearchResult
            {
                Id = h.Id,
                Score = h.Score,
                Content = h.Fields["content"] as string ?? string.Empty,
            }).ToList();
            return results;
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Failed to search vector database in namespace '{namespaceId}'", exception);
        }
    }

    public async Task Upsert(string namespaceId, List<VectorDocument> documents)
    {
        try
        {
            UpsertRequest request = new()
            {
                Namespace = namespaceId,
                Vectors = documents.Select(d => new Vector
                {
                    Id = d.Id,
                    Values = d.Vector,
                    Metadata = new(new Dictionary<string, MetadataValue?>
                    {
                        { "content", d.Content },
                        { "documentName", d.DocumentName },
                        { "embeddingText", d.EmbeddingText },
                    }),
                }).ToList(),
            };
            _ = await _index.UpsertAsync(request);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Failed to upsert {documents.Count} documents to vector database in namespace '{namespaceId}'", exception);
        }
    }

    public async Task DeleteByMetadata(string namespaceId, Metadata filter)
    {
        try
        {
            DeleteRequest request = new()
            {
                Namespace = namespaceId,
                Filter = filter
            };
            _ = await _index.DeleteAsync(request);
        }
        catch (PineconeApiException pineconeApiException)
        {
            if (pineconeApiException.Body is string bodyStr && bodyStr.Contains("StatusCode=\"NotFound\""))
            {
                // Ignore not found errors
                return;
            }
            throw;
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Failed to delete by metadata from vector database in namespace '{namespaceId}'", exception);
        }
    }

    public async Task Delete(string namespaceId, List<string> ids)
    {
        try
        {
            DeleteRequest request = new()
            {
                Namespace = namespaceId,
                Ids = ids,
            };
            _ = await _index.DeleteAsync(request);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Failed to delete {ids.Count} documents from vector database in namespace '{namespaceId}'", exception);
        }
    }

    public async Task Clear(string namespaceId)
    {
        try
        {
            DeleteRequest request = new()
            {
                Namespace = namespaceId,
                DeleteAll = true
            };
            _ = await _index.DeleteAsync(request);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Failed to clear vector database in namespace '{namespaceId}'", exception);
        }
    }
}