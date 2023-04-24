using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MPay.Infrastructure.ErrorHandling;

namespace MPay.Infrastructure.Tests.ErrorHandling;

public class ErrorHandlerMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ReturnErrorDetails_WhenExceptionOccurs()
    {
        // Arrange
        var errorDetails = CreateErrorDetails();
        var exceptionMapper = new Mock<IExceptionMapper>();
        exceptionMapper.Setup(x => x.Map(It.IsAny<Exception>())).Returns(errorDetails);
        var context = CreateMockHttpContext();
        var middleware = new ErrorHandlerMiddleware(exceptionMapper.Object);
        
        // Act
        await middleware.InvokeAsync(context, _ => throw new Exception());

        // Assert
        context.Response.StatusCode.Should().Be(errorDetails.Status);
        context.Response.ContentType.Should().Be("application/problem+json");
        context.Response.Body.Should().NotBeNull();
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var actualErrorDetails = JsonSerializer.Deserialize<ErrorDetails>(body);
        actualErrorDetails.Should().NotBeNull();
        actualErrorDetails.Should().BeEquivalentTo(errorDetails);
    }

    [Fact]
    public async Task InvokeAsync_ProcessNext_WhenNoExceptionOccurs()
    {
        // Arrange
        var exceptionMapper = new Mock<IExceptionMapper>();
        var context = CreateMockHttpContext();
        var requestDelegate = new Mock<RequestDelegate>();
        var middleware = new ErrorHandlerMiddleware(exceptionMapper.Object);
        
        // Act
        await middleware.InvokeAsync(context, requestDelegate.Object);
        
        // Assert
        requestDelegate.Verify(x => x.Invoke(It.IsAny<HttpContext>()), Times.Once);
    }
    
    private static HttpContext CreateMockHttpContext()
    {
        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };
        return context;
    }

    private static ErrorDetails CreateErrorDetails()
    {
        return new AutoFaker<ErrorDetails>()
            .RuleFor(x => x.Extensions, _ => null)
            .Generate();
    }
}