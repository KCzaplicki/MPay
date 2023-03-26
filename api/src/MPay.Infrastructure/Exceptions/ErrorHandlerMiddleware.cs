using Microsoft.AspNetCore.Http;

namespace MPay.Infrastructure.Exceptions;

internal class ErrorHandlerMiddleware : IMiddleware
{
    private readonly IExceptionMapper _mapper;

    public ErrorHandlerMiddleware(IExceptionMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(context, ex);
        }
    }

    private async Task HandleErrorAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        var problemDetails = _mapper.Map(exception);
        response.StatusCode = problemDetails.Status.Value;

        await response.WriteAsJsonAsync(problemDetails);
    }
}