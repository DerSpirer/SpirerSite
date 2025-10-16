using Backend.Api.Models.Core;
using Backend.Api.Models.OpenAI.Converters;
using Backend.Api.Models.OpenAI.OutputItems;
using Newtonsoft.Json;

namespace Backend.Api.Models.OpenAI.Responses;

public class Response
{
    public string? id { get; set; }
    public Status? status { get; set; }

    [JsonConverter(typeof(OutputItemListConverter))]
    public List<OutputItem>? output { get; set; }

    public ResponseError? error { get; set; }
    
    public class ResponseError
    {
        public string? code { get; set; }
        public string? message { get; set; }
    }
}

