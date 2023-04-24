using MPay.Abstractions.Exceptions;

namespace MPay.Infrastructure.Tests.ErrorHandling.Exceptions;

internal class MockWithDataException : MPayException
{
    public string Id { get; }
    public int Value { get; set; }
    
    public MockWithDataException(string message, string id, int value) : base(message)
    {
        Id = id;
        Value = value;
    }
}