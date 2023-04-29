using Microsoft.AspNetCore.Http;

namespace MPay.Tests.Shared.Common;

internal static class MockEndpointFilterInvocationContextFactory
{
    public static Mock<EndpointFilterInvocationContext> Create(object? argument = null)
    {
        var context = new Mock<EndpointFilterInvocationContext>();
        context.Setup(x => x.Arguments)
            .Returns(argument != null ? new[] { argument } : Array.Empty<object>());
        return context;
    }

}