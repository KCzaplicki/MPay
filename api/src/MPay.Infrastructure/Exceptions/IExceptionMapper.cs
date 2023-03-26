using Microsoft.AspNetCore.Mvc;

namespace MPay.Infrastructure.Exceptions;

public interface IExceptionMapper
{
    ProblemDetails Map(Exception exception);
}