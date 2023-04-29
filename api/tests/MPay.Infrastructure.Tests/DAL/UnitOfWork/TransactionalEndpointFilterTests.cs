using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MPay.Infrastructure.DAL.UnitOfWork;
using ContextFactory = MPay.Tests.Shared.Common.MockEndpointFilterInvocationContextFactory;
using NextFactory = MPay.Tests.Shared.Common.MockEndpointFilterDelegateFactory;

namespace MPay.Infrastructure.Tests.DAL.UnitOfWork;

public class TransactionalEndpointFilterTests
{
    private readonly ILogger<TransactionalEndpointFilter> _logger;

    public TransactionalEndpointFilterTests()
    {
        _logger = new Mock<ILogger<TransactionalEndpointFilter>>().Object;
    }
    
    [Fact]
    public async Task InvokeAsync_ExecutesAction()
    {
        // Arrange
        var actionExecuted = false;
        var context = ContextFactory.Create();
        var mockNext = NextFactory.Create();
        var mockUnitOfWork = CreateMockUnitOfWork(() => actionExecuted = true);
        var transactionalEndpointFilter = new TransactionalEndpointFilter(mockUnitOfWork.Object, _logger);
        
        // Act
        await transactionalEndpointFilter.InvokeAsync(context.Object, mockNext.Object);
        
        // Assert
        actionExecuted.Should().BeTrue();
        mockNext.Verify(f => f.Invoke(It.IsAny<EndpointFilterInvocationContext>()), Times.Once);
        mockUnitOfWork.Verify(u => u.ExecuteAsync(It.IsAny<Func<Task<object?>>>()), Times.Once);
    }
    
    [Fact]
    public async Task InvokeAsync_ReThrowsException()
    {
        // Arrange
        var context = ContextFactory.Create();
        var mockNext = NextFactory.Create();
        var mockUnitOfWork = CreateMockUnitOfWork(() => throw new Exception());
        var transactionalEndpointFilter = new TransactionalEndpointFilter(mockUnitOfWork.Object, _logger);
        
        // Act
        Func<Task> invokeAction = async () => await transactionalEndpointFilter.InvokeAsync(context.Object, mockNext.Object);
        
        // Assert
        await invokeAction.Should().ThrowAsync<Exception>();
        mockUnitOfWork.Verify(u => u.ExecuteAsync(It.IsAny<Func<Task<object?>>>()), Times.Once);
    }

    private static Mock<IUnitOfWork> CreateMockUnitOfWork(Action afterExecuteAction)
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        mockUnitOfWork.Setup(u => u.ExecuteAsync(It.IsAny<Func<Task<object?>>>()))
            .Callback<Func<Task<object?>>>(async executeExpression  =>
            {
                await executeExpression();
            })
            .ReturnsAsync(() =>
            {
                afterExecuteAction();
                return new object();
            });
        return mockUnitOfWork;
    }
}