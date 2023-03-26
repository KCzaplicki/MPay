using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MPay.Core.Exceptions;
using System.Collections.Concurrent;

namespace MPay.Infrastructure.Exceptions;

internal class ExceptionMapper : IExceptionMapper
{
    private const string InternalServerErrorMessage = "Unexpected error occurred.";
    private const string InternalServerErrorCode = "UNEXPECTED_ERROR";

    private static readonly ConcurrentDictionary<string, string> ErrorCodes = new();

    public ProblemDetails Map(Exception exception)
    {
        string errorCode, message;

        if (exception is MPayException mPayException)
        {
            errorCode = MapToErrorCode(mPayException.GetType().Name);
            message = mPayException.Message;
        }
        else
        {
            errorCode = InternalServerErrorCode;
            message = InternalServerErrorMessage;
        }

        var statusCode = exception switch
        {
            MPayException e when IsNotFoundException(e) => StatusCodes.Status404NotFound,
            MPayException _ => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        return new ProblemDetails
        {
            Title = errorCode,
            Detail = message,
            Status = statusCode
        };
    }

    private static bool IsNotFoundException(MPayException exception)
        => exception.GetType().Name.ToUpperInvariant().EndsWith("NOTFOUNDEXCEPTION");

    private static string MapToErrorCode(string exceptionName) 
        => ErrorCodes.GetOrAdd(exceptionName, exceptionName.Underscore().Replace("_exception", string.Empty).ToUpperInvariant());
}