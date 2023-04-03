namespace MPay.Abstractions.Exceptions;

public abstract class MPayException : Exception
{
    protected MPayException(string message) : base(message) { }
}