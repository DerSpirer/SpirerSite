using System.Runtime.CompilerServices;
using System.Text;
using Backend.Api.Enums;
using Backend.Api.Models.Agent;
using Backend.Api.Models.OpenAI.Requests;
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
    private static readonly JsonSerializer _serializer = JsonSerializer.Create(_serializerSettings);
    private static readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
    {
        Formatting = Formatting.None,
        NullValueHandling = NullValueHandling.Ignore,
        // The contract resolver is not needed because the properties are already snake_case
        // ContractResolver = new DefaultContractResolver()
        // {
        //     NamingStrategy = new SnakeCaseNamingStrategy()
        // }
    };
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

        throw new NotImplementedException();
    }
    private async Task<HttpResponseMessage> SendCreateResponseRequest(string input, string? previousResponseId = null, CancellationToken cancellationToken = default)
    {
        CreateResponseRequest request = new CreateResponseRequest
        {
            input = input,
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
}
