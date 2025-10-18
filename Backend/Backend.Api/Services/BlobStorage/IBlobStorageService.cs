namespace Backend.Api.Services.BlobStorage;

/// <summary>
/// Interface for blob storage operations
/// </summary>
public interface IBlobStorageService
{
    /// <summary>
    /// Downloads a blob as a string
    /// </summary>
    /// <param name="blobName">The name of the blob to download</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The content of the blob as a string</returns>
    Task<string> DownloadBlobAsStringAsync(string blobName, CancellationToken cancellationToken = default);
}

