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
}
