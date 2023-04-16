using Humanizer;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using MPay.Abstractions.Exceptions;

namespace MPay.Infrastructure.Exceptions;

internal class ExceptionMapper : IExceptionMapper
{
    private const string InternalServerErrorMessage = "Unexpected error occurred.";
    private const string InternalServerErrorCode = "UNEXPECTED_ERROR";
    private const string NotFoundExceptionSuffix = "NOTFOUNDEXCEPTION";

    private static readonly ConcurrentDictionary<string, string> ErrorCodes = new();

    public ErrorDetails Map(Exception exception) 
        => new()
        {
            Title = MapToTitle(exception),
            ErrorCode = MapToErrorCode(exception),
            Status = MapToStatusCode(exception),
            Data = MapToData(exception)
        };

    private Dictionary<string, object?> MapToData(Exception exception)
    {
        if (exception is MPayException)
        {
            return exception.GetType()
                            .GetProperties()
                            .Where(p => p.DeclaringType != typeof(Exception))
                            .ToDictionary(p => p.Name.Camelize(), p => p.GetValue(exception));
        }

        return null;
    }

    private static int MapToStatusCode(Exception exception) 
        => exception switch
        {
            MPayException e when IsNotFoundException(e) => StatusCodes.Status404NotFound,
            MPayException _ => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

    private static bool IsNotFoundException(MPayException exception)
        => exception.GetType().Name.ToUpperInvariant().EndsWith(NotFoundExceptionSuffix);

    private static string MapToTitle(Exception exception)
        => exception is MPayException mPayException ? mPayException.Message : InternalServerErrorMessage;

    private static string MapToErrorCode(Exception exception)
    {
        if (exception is not MPayException)
        {
            return InternalServerErrorCode;
        }

        var exceptionName = exception.GetType().Name;
        return ErrorCodes.GetOrAdd(exceptionName,
            exceptionName.Underscore().Replace("_exception", string.Empty).ToUpperInvariant());
    }
}