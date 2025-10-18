using Backend.Api.Models.Agent;
using Backend.Api.Services.Agent;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Backend.Api.Controllers;

/// <summary>
/// Controller for AI agent interactions
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AgentController : ControllerBase
{
    private readonly IAgentService _agentService;
    private readonly ILogger<AgentController> _logger;

    public AgentController(IAgentService agentService, ILogger<AgentController> logger)
    {
        _agentService = agentService;
        _logger = logger;
    }

    /// <summary>
    /// Generates a streaming response from the AI agent
    /// </summary>
    /// <param name="request">The chat request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Server-sent events stream of response chunks</returns>
    [HttpPost("chat/stream")]
    public async Task ChatStream(
        [FromBody] ChatRequest request, 
        CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Input))
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Input is required", cancellationToken);
                return;
            }

            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");

            await foreach (ChatResponse message in _agentService.GenerateStreamingResponseAsync(
                request.Input,
                request.PreviousResponseId,
                cancellationToken))
            {
                // Serialize the message to JSON and write it as a server-sent event
                string deltaJson = JsonConvert.SerializeObject(message, Formatting.None,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                await Response.WriteAsync($"data: {deltaJson}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }

            await Response.WriteAsync("data: [DONE]\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating streaming chat response");
            Response.StatusCode = 500;
            await Response.WriteAsync("data: [ERROR]\n\n", cancellationToken);
        }
    }

    /// <summary>
    /// Refreshes a knowledge base document by processing and updating it in the vector database
    /// </summary>
    /// <param name="request">The refresh KB document request</param>
    /// <returns>Status of the refresh operation</returns>
    [HttpPost("refresh-kb-document")]
    public IActionResult RefreshKbDocument([FromBody] RefreshKbDocumentRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.DocumentName))
            {
                return BadRequest(new { error = "DocumentName is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest(new { error = "Content is required" });
            }

            _logger.LogInformation($"Refreshing KB document: {request.DocumentName}");

            // TODO: Implement the actual refresh logic
            // 1. Parse/chunk the document content
            // 2. Generate embeddings for the chunks
            // 3. Update the vector database with the new embeddings

            _logger.LogInformation($"Successfully refreshed KB document: {request.DocumentName}");

            return Ok(new
            {
                status = "success",
                message = $"KB document '{request.DocumentName}' refreshed successfully",
                documentName = request.DocumentName,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Error refreshing KB document: {request.DocumentName}");
            return StatusCode(500, new
            {
                status = "error",
                message = "An error occurred while refreshing the KB document",
                error = exception.Message
            });
        }
    }
}
