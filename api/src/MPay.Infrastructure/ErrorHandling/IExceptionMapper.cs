namespace MPay.Infrastructure.ErrorHandling;

public interface IExceptionMapper
{
    ErrorDetails Map(Exception exception);
}