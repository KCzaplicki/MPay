using Microsoft.AspNetCore.Http;

namespace MPay.Tests.Shared.Common;

internal static class MockEndpointFilterDelegateFactory
{
    public static Mock<EndpointFilterDelegate> Create()
    {
        var endpointFilterDelegate = new Mock<EndpointFilterDelegate>();
        endpointFilterDelegate.Setup(f => f.Invoke(It.IsAny<EndpointFilterInvocationContext>())).ReturnsAsync(new object());
        return endpointFilterDelegate;
    }
}