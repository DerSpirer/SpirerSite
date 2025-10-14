namespace Backend.Api.Services.Embedding;

/// <summary>
/// Interface for generating text embeddings using OpenAI
/// </summary>
public interface IEmbeddingService
{
    /// <summary>
    /// Generates an embedding vector for the given text
    /// </summary>
    /// <param name="text">The text to generate an embedding for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A float array representing the embedding vector</returns>
    Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Generates embedding vectors for multiple texts
    /// </summary>
    /// <param name="texts">The texts to generate embeddings for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of float arrays representing the embedding vectors</returns>
    Task<IList<float[]>> GenerateEmbeddingsAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default);
}

