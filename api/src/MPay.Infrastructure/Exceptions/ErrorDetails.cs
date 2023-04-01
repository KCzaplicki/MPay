﻿using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace MPay.Infrastructure.Exceptions;

public class ErrorDetails : ProblemDetails
{
    [JsonPropertyName("errorCode")]
    public string ErrorCode { get; init; }

    [JsonPropertyName("data")]
    public IDictionary<string, object> Data { get; set; }
}