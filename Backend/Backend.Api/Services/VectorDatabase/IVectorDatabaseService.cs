using Backend.Api.Models.VectorDatabase;
using Pinecone;

namespace Backend.Api.Services.VectorDatabase;

public interface IVectorDatabaseService
{
    Task<List<VectorSearchResult>> Search(string namespaceId, string queryText, int topK = 10);
    Task Upsert(string namespaceId, List<VectorDocument> documents);
    Task Delete(string namespaceId, List<string> ids);
    Task DeleteByMetadata(string namespaceId, Metadata filter);
    Task Clear(string namespaceId);
}