using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Humanizer;
using Microsoft.AspNetCore.Http;

namespace MPay.Infrastructure.Validation;

public class ValidationErrorDetails : ProblemDetails
{
    [JsonPropertyName("errorCode")] 
    public string ErrorCode => 
        GetType().Name.Underscore().Replace("_details", string.Empty).ToUpperInvariant();

    [JsonPropertyName("errors")]
    public IDictionary<string, ValidationError[]> Errors { get; init; }

    public ValidationErrorDetails()
    {
        Title = "One or more validation errors occurred.";
        Status = StatusCodes.Status400BadRequest;
    }
}