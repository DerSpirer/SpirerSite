using Backend.Api.Models.Agent;

namespace Backend.Api.Services.Agent;

/// <summary>
/// Interface for AI agent interactions using OpenAI
/// </summary>
public interface IAgentService
{
    /// <summary>
    /// Generates a streaming completion response from the agent
    /// </summary>
    /// <param name="input">The input string to send to the agent</param>
    /// <param name="previousResponseId">Optional previous response ID for context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An async enumerable of response chunks as strings</returns>
    IAsyncEnumerable<ChatResponse> GenerateStreamingResponseAsync(
        string input,
        string? previousResponseId = null,
        CancellationToken cancellationToken = default);
}

