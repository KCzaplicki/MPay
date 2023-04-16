using Microsoft.AspNetCore.Http;

namespace MPay.Infrastructure.ErrorHandling;

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
        var errorDetails = _mapper.Map(exception);
        response.StatusCode = errorDetails.Status.Value;

        await response.WriteAsJsonAsync(errorDetails);
    }
}