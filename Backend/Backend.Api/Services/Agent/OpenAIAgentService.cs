using System.Runtime.CompilerServices;
using System.Text;
using Backend.Api.Enums;
using Backend.Api.Models.Agent;
using Backend.Api.Models.OpenAI.Requests;
using Backend.Api.Models.OpenAI.Objects;
using Backend.Api.Models.OpenAI.Objects.StreamingEvents;
using Backend.Api.Models.OpenAI.Converters;
using Backend.Api.Services.KnowledgeBase;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace Backend.Api.Services.Agent;

/// <summary>
/// Buffer for accumulating function call details during streaming
/// </summary>
internal class FunctionCallBuffer
{
    public string? CallId { get; set; }
    public string? Name { get; set; }
    public string? Arguments { get; set; }
    public bool IsComplete { get; set; }
}

/// <summary>
/// State tracking for stream processing
/// </summary>
internal class StreamProcessingState
{
    public string? CurrentResponseId { get; set; }
    public FunctionCallBuffer? FunctionCallBuffer { get; set; }
}

/// <summary>
/// Service for AI agent interactions using OpenAI Chat Completions API
/// </summary>
public class OpenAIAgentService : IAgentService
{
    private const string BASE_URL = "https://api.openai.com/v1";

    private readonly ILogger<OpenAIAgentService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IKnowledgeBaseService _knowledgeBaseService;
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

    public OpenAIAgentService(
        IConfiguration configuration, 
        ILogger<OpenAIAgentService> logger, 
        HttpClient httpClient,
        IKnowledgeBaseService knowledgeBaseService)
    {
        _logger = logger;
        _httpClient = httpClient;
        _knowledgeBaseService = knowledgeBaseService;
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

        var inputItems = new List<InputItem> 
        { 
            new InputMessage { type = "message", role = MessageRole.User, content = input } 
        };

        await foreach (var chatResponse in GenerateStreamingResponseInternalAsync(inputItems, previousResponseId, cancellationToken))
        {
            yield return chatResponse;
        }
    }

    private async IAsyncEnumerable<ChatResponse> GenerateStreamingResponseInternalAsync(
        List<InputItem> inputItems,
        string? previousResponseId = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponse = await SendCreateResponseRequest(inputItems, previousResponseId, cancellationToken);

        // Read the stream using OpenAIStreamReader and yield response chunks as they arrive
        Stream stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);

        var state = new StreamProcessingState();

        await foreach (var (chatResponse, shouldYield) in ProcessStreamAsync(stream, state, cancellationToken))
        {
            if (shouldYield && chatResponse != null)
            {
                yield return chatResponse;
            }
        }

        // If we buffered a function call, execute it and generate a new response
        if (state.FunctionCallBuffer != null && state.FunctionCallBuffer.IsComplete)
        {
            _logger.LogInformation("Executing buffered function call: {FunctionName}", state.FunctionCallBuffer.Name);
            
            // Execute the tool call
            string toolResult = await ExecuteToolCallAsync(state.FunctionCallBuffer.Name!, state.FunctionCallBuffer.Arguments!, cancellationToken);
            
            _logger.LogInformation("Tool call executed, result: {ToolResult}", toolResult);

            // Create a new input list with the function call output
            var newInputItems = new List<InputItem>(inputItems)
            {
                new FunctionToolCallOutput
                {
                    type = "function_call_output",
                    call_id = state.FunctionCallBuffer.CallId!,
                    output = toolResult
                }
            };

            // Generate a new response with the tool result
            // Note: The new response will have a different ID, and the frontend will automatically
            // update to use this new ID for subsequent requests via the ResponseEvent stream
            _logger.LogInformation("Generating new response with tool result, previous response ID: {PreviousResponseId}", state.CurrentResponseId);
            await foreach (var chatResponse in GenerateStreamingResponseInternalAsync(newInputItems, state.CurrentResponseId, cancellationToken))
            {
                yield return chatResponse;
            }
        }
    }
    private async Task<HttpResponseMessage> SendCreateResponseRequest(List<InputItem> inputItems, string? previousResponseId = null, CancellationToken cancellationToken = default)
    {
        CreateResponseRequest request = new CreateResponseRequest
        {
            input = inputItems,
            previous_response_id = previousResponseId,
            tools = new List<Tool>
            {
                new FunctionTool
                {
                    name = "query_knowledge_base",
                    description = "Search the knowledge base for relevant information about Tom Spirer's background, projects, and experience. Use this when you need to retrieve specific information about Tom.",
                    parameters = JSchema.Parse(@"{
                        ""type"": ""object"",
                        ""properties"": {
                            ""query"": {
                                ""type"": ""string"",
                                ""description"": ""The search query to find relevant information in the knowledge base""
                            }
                        },
                        ""required"": [""query""],
                        ""additionalProperties"": false
                    }"),
                    strict = true
                }
            }
        };

        string payload = Serialize(request);
        HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{BASE_URL}/responses")
        {
            Headers = {
                { "Authorization", $"Bearer {_apiKey}" },
            },
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
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

    private async IAsyncEnumerable<(ChatResponse? chatResponse, bool shouldYield)> ProcessStreamAsync(
        Stream stream,
        StreamProcessingState state,
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

            // Capture response ID for chaining and send it to frontend
            // The frontend will update its previousResponseId to this value
            if (streamingEvent is ResponseEvent responseEvent)
            {
                state.CurrentResponseId = responseEvent.response.id;
                _logger.LogInformation("Response event received, ID: {ResponseId}, Status: {Status}", 
                    responseEvent.response.id, responseEvent.response.status);
            }

            // Handle output item events to capture function name
            if (streamingEvent is ResponseOutputItemEvent outputItemEvent)
            {
                // Check if this is a function call output item
                if (outputItemEvent.item is FunctionToolCall functionToolCall && 
                    streamingEvent.type == "response.output_item.added")
                {
                    _logger.LogInformation("Received function call output item: {FunctionName}", functionToolCall.name);
                    
                    // Initialize the buffer with the function name and call_id
                    state.FunctionCallBuffer = new FunctionCallBuffer
                    {
                        CallId = functionToolCall.call_id,
                        Name = functionToolCall.name
                    };
                    
                    // Don't yield function call output items to frontend
                    continue;
                }
            }

            // Handle function call arguments events - buffer them instead of yielding
            if (streamingEvent is ResponseFunctionCallArgumentsEvent functionCallEvent)
            {
                _logger.LogInformation("Received function call arguments event: {EventType}", streamingEvent.type);
                
                if (state.FunctionCallBuffer == null)
                {
                    _logger.LogWarning("Received function call arguments without output item");
                    state.FunctionCallBuffer = new FunctionCallBuffer
                    {
                        CallId = functionCallEvent.item_id
                    };
                }

                // Accumulate arguments
                if (!string.IsNullOrEmpty(functionCallEvent.delta))
                {
                    state.FunctionCallBuffer.Arguments += functionCallEvent.delta;
                }

                // Mark as complete when we get the .done event
                if (streamingEvent.type == "response.function_call_arguments.done")
                {
                    state.FunctionCallBuffer.Arguments = functionCallEvent.arguments ?? state.FunctionCallBuffer.Arguments;
                    state.FunctionCallBuffer.IsComplete = true;
                    _logger.LogInformation("Function call buffered - Name: {Name}, Args: {Args}", 
                        state.FunctionCallBuffer.Name, state.FunctionCallBuffer.Arguments);
                }

                // Don't yield function call arguments events to frontend
                continue;
            }

            // Map streaming event to ChatResponse and yield to frontend
            ChatResponse? chatResponse = MapStreamingEventToChatResponse(streamingEvent);
            
            if (chatResponse != null)
            {
                yield return (chatResponse, true);
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
                Status = responseEvent.response.status
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
            
            // Function calls are handled separately in ProcessStreamAsync
            // For output item and content part events, we don't need to yield anything yet
            ResponseOutputItemEvent => null,
            ResponseContentPartEvent => null,
            
            _ => null
        };
    }

    /// <summary>
    /// Executes a tool call based on the function name and arguments
    /// </summary>
    /// <param name="functionName">The name of the function to execute</param>
    /// <param name="arguments">The JSON arguments for the function</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the function execution as a JSON string</returns>
    private async Task<string> ExecuteToolCallAsync(string functionName, string arguments, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Executing tool call: {FunctionName} with arguments: {Arguments}", functionName, arguments);

        try
        {
            switch (functionName)
            {
                case "query_knowledge_base":
                    return await ExecuteQueryKnowledgeBaseAsync(arguments, cancellationToken);
                
                default:
                    _logger.LogWarning("Unknown function called: {FunctionName}", functionName);
                    return JsonConvert.SerializeObject(new { error = $"Unknown function: {functionName}" });
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error executing tool call: {FunctionName}", functionName);
            return JsonConvert.SerializeObject(new { error = exception.Message });
        }
    }

    /// <summary>
    /// Executes the query_knowledge_base function
    /// </summary>
    private async Task<string> ExecuteQueryKnowledgeBaseAsync(string arguments, CancellationToken cancellationToken = default)
    {
        const int TOP_K = 10;
        
        try
        {
            JObject argsObj = JObject.Parse(arguments);
            string query = argsObj["query"]?.ToString() ?? throw new ArgumentException("query parameter is required");

            var results = await _knowledgeBaseService.QueryAsync(query, TOP_K, cancellationToken);

            // Format results as a readable string for the model
            var formattedResults = results.Select(r => new
            {
                content = r.Content,
                score = r.Score,
                id = r.Id
            });

            return JsonConvert.SerializeObject(new
            {
                results = formattedResults,
                count = results.Count
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error executing query_knowledge_base");
            throw;
        }
    }
}
