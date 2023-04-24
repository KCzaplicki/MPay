using MPay.Abstractions.Exceptions;

namespace MPay.Infrastructure.Tests.ErrorHandling.Exceptions;

internal class MockNotFoundException : MPayException
{
    public MockNotFoundException(string message) : base(message)
    {
    }
}