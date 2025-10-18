using Azure.Storage.Blobs;
using Backend.Api.Enums;

namespace Backend.Api.Services.BlobStorage;

/// <summary>
/// Service for Azure Blob Storage operations
/// </summary>
public class AzureBlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<AzureBlobStorageService> _logger;
    private readonly string _containerName;

    public AzureBlobStorageService(
        IConfiguration configuration,
        ILogger<AzureBlobStorageService> logger)
    {
        _logger = logger;
        
        string connectionString = configuration[VaultKey.AzureBlobStorageConnectionString]
            ?? throw new InvalidOperationException("Azure Blob Storage connection string is missing");
        
        _containerName = configuration["AzureBlobStorage:ContainerName"] ?? "agent-prompts";
        
        _blobServiceClient = new BlobServiceClient(connectionString);
        
        _logger.LogInformation("AzureBlobStorageService initialized with container: {ContainerName}", _containerName);
    }

    public async Task<string> DownloadBlobAsStringAsync(string blobName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Downloading blob: {BlobName} from container: {ContainerName}", blobName, _containerName);
            
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync(cancellationToken))
            {
                _logger.LogWarning("Blob not found: {BlobName}", blobName);
                throw new FileNotFoundException($"Blob '{blobName}' not found in container '{_containerName}'");
            }

            var response = await blobClient.DownloadContentAsync(cancellationToken);
            string content = response.Value.Content.ToString();
            
            _logger.LogInformation("Successfully downloaded blob: {BlobName}, size: {Size} characters", blobName, content.Length);
            
            return content;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error downloading blob: {BlobName}", blobName);
            throw;
        }
    }
}

