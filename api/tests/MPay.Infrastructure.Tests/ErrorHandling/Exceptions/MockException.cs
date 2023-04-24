using MPay.Abstractions.Exceptions;

namespace MPay.Infrastructure.Tests.ErrorHandling.Exceptions;

internal class MockException : MPayException
{
    public MockException(string message) : base(message)
    {
    }
}