using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Backend.Functions.Functions;

public class UpdateAgentKbBlob
{
    private readonly ILogger<UpdateAgentKbBlob> _logger;

    public UpdateAgentKbBlob(ILogger<UpdateAgentKbBlob> logger)
    {
        _logger = logger;
    }

    [Function(nameof(UpdateAgentKbBlob))]
    public async Task Run([BlobTrigger("agent-kb-blobs/{name}")] Stream stream, string name)
    {
        using var blobStreamReader = new StreamReader(stream);
        var content = await blobStreamReader.ReadToEndAsync();
        _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
        
    }
}