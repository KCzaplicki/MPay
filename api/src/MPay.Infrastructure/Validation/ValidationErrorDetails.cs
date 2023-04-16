using System.Text.Json.Serialization;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MPay.Infrastructure.Validation;

public class ValidationErrorDetails : ProblemDetails
{
    public ValidationErrorDetails()
    {
        Title = "One or more validation errors occurred.";
        Status = StatusCodes.Status400BadRequest;
    }

    [JsonPropertyName("errorCode")]
    public string ErrorCode =>
        GetType().Name.Underscore().Replace("_details", string.Empty).ToUpperInvariant();

    [JsonPropertyName("errors")] public IDictionary<string, ValidationError[]> Errors { get; init; }
}