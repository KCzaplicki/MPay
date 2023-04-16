using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace MPay.Infrastructure.ErrorHandling;

public class ErrorDetails : ProblemDetails
{
    [JsonPropertyName("errorCode")] public string ErrorCode { get; init; }

    [JsonPropertyName("data")] public IDictionary<string, object> Data { get; set; }
}