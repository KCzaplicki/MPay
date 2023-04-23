using MPay.Core.Entities;
using MPay.Infrastructure.DAL;
using MPay.Tests.Shared.DAL;
using UnitOfWorkService = MPay.Infrastructure.DAL.UnitOfWork.UnitOfWork;

namespace MPay.Infrastructure.Tests.DAL.UnitOfWork;

public class UnitOfWorkTests
{
    [Fact]
    public async Task ExecuteAsync_ExecutesAction()
    {
        // Arrange
        var actionExecuted = false;
        var context = MockDbContextFactory.Create<MPayDbContext>();
        var unitOfWork = new UnitOfWorkService(context);
        
        // Act
        var result = await unitOfWork.ExecuteAsync(async () =>
        {
            actionExecuted = true;
            return true;
        });
        
        // Assert
        actionExecuted.Should().BeTrue();
        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task ExecuteAsync_ReThrowsException()
    {
        // Arrange
        var context = MockDbContextFactory.Create<MPayDbContext>();
        var unitOfWork = new UnitOfWorkService(context);
        
        // Act
        Func<Task<bool>> unitOfWorkAction = async () => await unitOfWork.ExecuteAsync<bool>(async () => throw new Exception());
        
        // Assert
        unitOfWorkAction.Should().ThrowAsync<Exception>();
    }
}