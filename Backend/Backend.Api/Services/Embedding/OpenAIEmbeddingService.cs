using Backend.Api.Enums;
using OpenAI;
using OpenAI.Embeddings;

namespace Backend.Api.Services.Embedding;

/// <summary>
/// Service for generating text embeddings using OpenAI API
/// </summary>
public class OpenAIEmbeddingService : IEmbeddingService
{
    private const string DefaultEmbeddingModel = "text-embedding-3-small";
    
    private readonly EmbeddingClient _embeddingClient;
    private readonly ILogger<OpenAIEmbeddingService> _logger;

    public OpenAIEmbeddingService(IConfiguration configuration, ILogger<OpenAIEmbeddingService> logger)
    {
        _logger = logger;
        
        var apiKey = configuration[VaultKey.OpenAiApiKey] 
            ?? throw new InvalidOperationException("OpenAI:ApiKey configuration is missing");
        
        _embeddingClient = new EmbeddingClient(DefaultEmbeddingModel, apiKey);
        
        _logger.LogInformation("OpenAIEmbeddingService initialized.");
    }

    /// <inheritdoc />
    public async Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text cannot be null or whitespace", nameof(text));
            }

            _logger.LogDebug("Generating embedding for text of length {Length}", text.Length);
            
            var embedding = await _embeddingClient.GenerateEmbeddingAsync(text, cancellationToken: cancellationToken);
            
            var vector = embedding.Value.ToFloats().ToArray();
            
            _logger.LogDebug("Generated embedding with {Dimensions} dimensions", vector.Length);
            
            return vector;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate embedding for text");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IList<float[]>> GenerateEmbeddingsAsync(IEnumerable<string> texts, CancellationToken cancellationToken = default)
    {
        try
        {
            var textList = texts.ToList();
            
            if (textList.Count == 0)
            {
                return Array.Empty<float[]>();
            }

            if (textList.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("Texts collection contains null or whitespace entries", nameof(texts));
            }

            _logger.LogDebug("Generating embeddings for {Count} texts", textList.Count);
            
            var embeddings = await _embeddingClient.GenerateEmbeddingsAsync(textList, cancellationToken: cancellationToken);
            
            var vectors = embeddings.Value
                .Select(e => e.ToFloats().ToArray())
                .ToList();
            
            _logger.LogDebug("Generated {Count} embeddings", vectors.Count);
            
            return vectors;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate embeddings for multiple texts");
            throw;
        }
    }
}

