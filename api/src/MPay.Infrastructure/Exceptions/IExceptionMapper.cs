namespace MPay.Infrastructure.Exceptions;

public interface IExceptionMapper
{
    ErrorDetails Map(Exception exception);
}