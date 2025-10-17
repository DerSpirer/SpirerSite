using System.Runtime.CompilerServices;
using System.Text;
using Backend.Api.Enums;
using Backend.Api.Models.Agent;
using Backend.Api.Models.OpenAI.Requests;
using Backend.Api.Models.OpenAI.Responses.StreamingEvents;
using Backend.Api.Models.OpenAI.Converters;
using Newtonsoft.Json;

namespace Backend.Api.Services.Agent;

/// <summary>
/// Service for AI agent interactions using OpenAI Chat Completions API
/// </summary>
public class OpenAIAgentService : IAgentService
{
    private const string BASE_URL = "https://api.openai.com/v1";

    private readonly ILogger<OpenAIAgentService> _logger;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
    {
        Formatting = Formatting.None,
        NullValueHandling = NullValueHandling.Ignore,
        Converters = new List<JsonConverter> { new StreamingEventConverter() }
        // The contract resolver is not needed because the properties are already snake_case
        // ContractResolver = new DefaultContractResolver()
        // {
        //     NamingStrategy = new SnakeCaseNamingStrategy()
        // }
    };
    private static readonly JsonSerializer _serializer = JsonSerializer.Create(_serializerSettings);
    private readonly string _apiKey;

    public OpenAIAgentService(IConfiguration configuration, ILogger<OpenAIAgentService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
        _apiKey = configuration[VaultKey.OpenAiApiKey]
            ?? throw new InvalidOperationException("OpenAI ApiKey configuration is missing");
        _logger.LogInformation("OpenAIAgentService initialized.");
    }

    public async IAsyncEnumerable<ChatResponse> GenerateStreamingResponseAsync(
        string input,
        string? previousResponseId = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating streaming response for input with previous response ID: {PreviousResponseId}", previousResponseId);

        HttpResponseMessage httpResponse = await SendCreateResponseRequest(input, previousResponseId, cancellationToken);

        // Read the stream using OpenAIStreamReader and yield response chunks as they arrive
        Stream stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);

        await foreach (var chatResponse in ProcessStreamAsync(stream, cancellationToken))
        {
            yield return chatResponse;
        }
    }
    private async Task<HttpResponseMessage> SendCreateResponseRequest(string input, string? previousResponseId = null, CancellationToken cancellationToken = default)
    {
        CreateResponseRequest request = new CreateResponseRequest
        {
            input = new List<InputMessage> { new InputMessage { role = MessageRole.User, content = input } },
            previous_response_id = previousResponseId
        };

        HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{BASE_URL}/responses")
        {
            Headers = {
                { "Authorization", $"Bearer {_apiKey}" },
            },
            Content = new StringContent(Serialize(request), Encoding.UTF8, "application/json")
        };

        HttpResponseMessage httpResponse = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        if (!httpResponse.IsSuccessStatusCode)
        {
            string errorContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Failed to generate streaming response: {StatusCode} {ErrorContent}", httpResponse.StatusCode, errorContent);
            throw new HttpRequestException($"Failed to generate streaming response: {httpResponse.StatusCode} {errorContent}");
        }

        return httpResponse;
    }
    private string Serialize(object obj)
    {
        using var stringWriter = new StringWriter();
        using var jsonWriter = new JsonTextWriter(stringWriter);
        _serializer.Serialize(jsonWriter, obj);
        return stringWriter.ToString();
    }

    private async IAsyncEnumerable<ChatResponse> ProcessStreamAsync(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8);
        
        while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
        {
            string? line = await reader.ReadLineAsync(cancellationToken);
            _logger.LogDebug(line);
            
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            // SSE format: lines start with "data: "
            if (!line.StartsWith("data: "))
            {
                continue;
            }

            string data = line.Substring(6); // Remove "data: " prefix

            // Check for stream end
            if (data == "[DONE]")
            {
                _logger.LogInformation("Stream completed with [DONE] signal");
                yield break;
            }

            // Deserialize the event
            StreamingEvent? streamingEvent;
            try
            {
                streamingEvent = JsonConvert.DeserializeObject<StreamingEvent>(data, _serializerSettings);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to deserialize streaming event: {Data}", data);
                continue;
            }

            if (streamingEvent == null)
            {
                _logger.LogWarning("Received null streaming event for data: {Data}", data);
                continue;
            }

            // Map streaming event to ChatResponse
            ChatResponse? chatResponse = MapStreamingEventToChatResponse(streamingEvent);
            
            if (chatResponse != null)
            {
                yield return chatResponse;
            }
        }
    }

    private ChatResponse? MapStreamingEventToChatResponse(StreamingEvent streamingEvent)
    {
        return streamingEvent switch
        {
            ResponseEvent responseEvent => new ChatResponse
            {
                Id = responseEvent.response.id,
                Status = Enum.TryParse<Status>(responseEvent.response.status, out var status) ? status : null
            },
            
            ResponseOutputTextEvent outputTextEvent => new ChatResponse
            {
                Content = outputTextEvent.delta ?? outputTextEvent.text
            },
            
            ResponseRefusalEvent refusalEvent => new ChatResponse
            {
                Refusal = refusalEvent.delta ?? refusalEvent.refusal
            },
            
            ResponseReasoningTextEvent reasoningEvent => new ChatResponse
            {
                Reasoning = reasoningEvent.delta ?? reasoningEvent.text
            },
            
            ResponseFunctionCallArgumentsEvent functionCallEvent => new ChatResponse
            {
                ToolCallId = functionCallEvent.item_id,
                ToolName = functionCallEvent.name,
                ToolArguments = functionCallEvent.delta ?? functionCallEvent.arguments
            },
            
            // For output item and content part events, we don't need to yield anything yet
            ResponseOutputItemEvent => null,
            ResponseContentPartEvent => null,
            
            _ => null
        };
    }
}
