using System.Runtime.CompilerServices;
using Backend.Api.Models.Core;
using Backend.Api.Models.OpenAI.OutputItems;
using Backend.Api.Models.OpenAI.Streaming;
using Newtonsoft.Json;

namespace Backend.Api.Services.Agent;

/// <summary>
/// Reads and parses OpenAI Server-Sent Events (SSE) streams and translates them into accumulated Message objects
/// </summary>
public class OpenAIStreamReader : IDisposable
{
    private readonly Stream _stream;
    private readonly StreamReader _reader;
    private readonly ILogger _logger;

    // Accumulator for message properties
    private readonly Message _message = new();

    public OpenAIStreamReader(Stream stream, ILogger logger)
    {
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        _reader = new StreamReader(_stream);
        _logger = logger;
    }

    /// <summary>
    /// Reads the stream asynchronously and yields accumulated Message objects as they build up
    /// </summary>
    public async IAsyncEnumerable<Message> ReadStreamAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? line;

        while ((line = await _reader.ReadLineAsync()) != null)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            // Lines starting with "data:" contain the JSON payload
            if (line.StartsWith("data:"))
            {
                string data = line.Substring("data:".Length).Trim();
                
                // Skip empty data or end-of-stream markers
                if (string.IsNullOrWhiteSpace(data) || data == "[DONE]")
                {
                    continue;
                }

                // Parse and process the event
                bool shouldYield = ProcessEvent(data);
                if (shouldYield)
                {
                    yield return _message;
                }
            }
        }
    }

    /// <summary>
    /// Processes an individual SSE event and returns true if we should yield the message
    /// </summary>
    private bool ProcessEvent(string jsonData)
    {
        try
        {
            var baseEvent = JsonConvert.DeserializeObject<StreamEvent>(jsonData);
            if (baseEvent == null) return false;

            switch (baseEvent.type)
            {
                case "response.output_item.added":
                    return ProcessOutputItemAdded(JsonConvert.DeserializeObject<OutputItemEvent>(jsonData));
                
                case "response.output_text.delta":
                    return ProcessTextDelta(JsonConvert.DeserializeObject<TextDeltaEvent>(jsonData));
                
                case "response.output_item.done":
                    return ProcessOutputItemDone(JsonConvert.DeserializeObject<OutputItemEvent>(jsonData));
                
                case "response.completed":
                    _message.status = Status.completed;
                    return true;
                
                default:
                    return false;
            }
        }
        catch (Exception exception)
        {
            _logger?.LogError(exception, "Error processing event: {JsonData}", jsonData);
            return false;
        }
    }

    private bool ProcessOutputItemAdded(OutputItemEvent? evt)
    {
        if (evt?.item == null) return false;

        // Set id and status from the output item
        _message.id = evt.item.id;
        _message.status = evt.item.status;

        // Set properties based on output item type
        switch (evt.item)
        {
            case MessageOutputItem messageItem:
                _message.Role = messageItem.role;
                break;

            case FunctionCallOutputItem functionItem:
                _message.ToolCallId = functionItem.call_id;
                _message.ToolName = functionItem.name;
                break;
        }

        return true;
    }

    private bool ProcessTextDelta(TextDeltaEvent? evt)
    {
        if (evt == null || string.IsNullOrEmpty(evt.delta)) return false;

        // Accumulate text delta based on output type
        if (evt.output_index == 0) // Assuming first output item
        {
            if (evt.type == "response.output_text.delta")
            {
                // This is content text
                _message.Content = (_message.Content ?? string.Empty) + evt.delta;
            }
        }

        return true;
    }

    private bool ProcessOutputItemDone(OutputItemEvent? evt)
    {
        if (evt?.item == null) return false;

        // Update status
        _message.status = evt.item.status;

        // Handle additional properties based on item type
        switch (evt.item)
        {
            case FunctionCallOutputItem functionItem:
                _message.ToolArguments = functionItem.arguments;
                break;

            case ReasoningOutputItem reasoningItem:
                if (reasoningItem.content != null && reasoningItem.content.Count > 0)
                {
                    var reasoningParts = reasoningItem.content
                        .Where(c => !string.IsNullOrEmpty(c.text))
                        .Select(c => c.text);
                    _message.Reasoning = string.Join("", reasoningParts);
                }
                break;

            case MessageOutputItem messageItem:
                if (messageItem.content != null && messageItem.content.Count > 0)
                {
                    // Check for refusal
                    var refusal = messageItem.content.FirstOrDefault(c => !string.IsNullOrEmpty(c.refusal))?.refusal;
                    if (!string.IsNullOrEmpty(refusal))
                    {
                        _message.Refusal = refusal;
                    }
                }
                break;
        }

        return true;
    }

    public void Dispose()
    {
        _reader?.Dispose();
        _stream?.Dispose();
    }
}

