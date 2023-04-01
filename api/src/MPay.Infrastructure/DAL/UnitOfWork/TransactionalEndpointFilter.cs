using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace MPay.Infrastructure.DAL.UnitOfWork;

internal class TransactionalEndpointFilter : IEndpointFilter
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionalEndpointFilter> _logger;

    public TransactionalEndpointFilter(IUnitOfWork unitOfWork, ILogger<TransactionalEndpointFilter> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
           return await _unitOfWork.ExecuteAsync(async () => await next(context));
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Exception occurred. Rolling back transaction.");
            throw;
        }
    }
}