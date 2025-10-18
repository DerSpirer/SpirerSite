using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Backend.Functions.Functions;

public class UpdateAgentKbBlob
{
    private readonly ILogger<UpdateAgentKbBlob> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public UpdateAgentKbBlob(
        ILogger<UpdateAgentKbBlob> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [Function(nameof(UpdateAgentKbBlob))]
    public async Task Run([BlobTrigger("agent-kb-blobs/{name}")] Stream stream, string name)
    {
        try
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name}");

            // Get backend API URL from configuration
            var backendApiUrl = _configuration["BackendApiUrl"] ?? throw new InvalidOperationException("BackendApiUrl is not configured");

            // Prepare the request payload
            var payload = new
            {
                documentName = name,
                content = content
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            // Send HTTP request to backend API
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync(
                $"{backendApiUrl}/api/agent/refresh-kb-document",
                jsonContent);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Successfully refreshed KB document: {name}");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to refresh KB document: {name}. Status: {response.StatusCode}, Error: {errorContent}");
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Error processing blob: {name}");
            throw;
        }
    }
}