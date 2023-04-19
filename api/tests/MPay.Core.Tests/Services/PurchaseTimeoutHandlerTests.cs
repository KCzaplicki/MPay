using Microsoft.Extensions.Logging;
using Moq;
using MPay.Abstractions.Events;
using MPay.Core.Entities;
using MPay.Core.Events;
using MPay.Core.Policies.PurchaseTimeout;
using MPay.Core.Services;
using MPay.Tests.Shared.Repositories;

namespace MPay.Core.Tests.Services;

public class PurchaseTimeoutHandlerTests
{
    private readonly Mock<ILogger<PurchaseTimeoutHandler>> _logger;

    public PurchaseTimeoutHandlerTests()
    {
        _logger = new Mock<ILogger<PurchaseTimeoutHandler>>();
    }

    [Fact]
    public async Task ExecuteAsync_ProcessPurchases_When_TimeoutPurchasesExists()
    {
        // Arrange
        var purchases = AutoFaker.Generate<List<Purchase>>();
        var mockPurchaseRepository = MockPurchaseRepositoryFactory.Create(purchases);
        var purchaseTimeoutPolicies = CreateMockPurchaseTimeoutPolicies(true);
        var asyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchaseTimeoutHandler = new PurchaseTimeoutHandler(mockPurchaseRepository.Object, purchaseTimeoutPolicies,
            asyncEventDispatcher.Object, _logger.Object);

        // Act
        await purchaseTimeoutHandler.ExecuteAsync();

        // Assert
        mockPurchaseRepository.Verify(x => x.UpdateAsync(It.IsAny<Purchase>()), Times.Exactly(purchases.Count));
        foreach (var purchase in purchases) mockPurchaseRepository.Verify(x => x.UpdateAsync(purchase), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotProcessPurchases_When_TimeoutPurchasesDoesNotExist()
    {
        // Arrange
        var purchases = AutoFaker.Generate<List<Purchase>>();
        var mockPurchaseRepository = MockPurchaseRepositoryFactory.Create(purchases);
        var purchaseTimeoutPolicies = CreateMockPurchaseTimeoutPolicies(false);
        var asyncEventDispatcher = new Mock<IAsyncEventDispatcher>();
        var purchaseTimeoutHandler = new PurchaseTimeoutHandler(mockPurchaseRepository.Object, purchaseTimeoutPolicies,
            asyncEventDispatcher.Object, _logger.Object);

        // Act
        await purchaseTimeoutHandler.ExecuteAsync();

        // Assert
        mockPurchaseRepository.Verify(x => x.UpdateAsync(It.IsAny<Purchase>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_PublishesPurchaseTimeoutEvent_When_TimeoutPurchasesExists()
    {
        // Arrange
        var purchases = AutoFaker.Generate<List<Purchase>>();
        var mockPurchaseRepository = MockPurchaseRepositoryFactory.Create(purchases);
        var purchaseTimeoutPolicies = CreateMockPurchaseTimeoutPolicies(true);
        var asyncEventDispatcher = new Mock<IAsyncEventDispatcher>();
        var purchaseTimeoutHandler = new PurchaseTimeoutHandler(mockPurchaseRepository.Object, purchaseTimeoutPolicies,
            asyncEventDispatcher.Object, _logger.Object);

        // Act
        await purchaseTimeoutHandler.ExecuteAsync();

        // Assert
        asyncEventDispatcher.Verify(x => x.PublishAsync(It.IsAny<PurchaseTimeout>()), Times.Exactly(purchases.Count));
        foreach (var purchase in purchases)
            asyncEventDispatcher.Verify(x => x.PublishAsync(It.Is<PurchaseTimeout>(
                e => e.Id == purchase.Id && e.TimeoutAt == purchase.CompletedAt)), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_DoesNotPublishPurchaseTimeoutEvent_When_TimeoutPurchasesDoesNotExist()
    {
        // Arrange
        var purchases = AutoFaker.Generate<List<Purchase>>();
        var mockPurchaseRepository = MockPurchaseRepositoryFactory.Create(purchases);
        var purchaseTimeoutPolicies = CreateMockPurchaseTimeoutPolicies(false);
        var asyncEventDispatcher = new Mock<IAsyncEventDispatcher>();
        var purchaseTimeoutHandler = new PurchaseTimeoutHandler(mockPurchaseRepository.Object, purchaseTimeoutPolicies,
            asyncEventDispatcher.Object, _logger.Object);

        // Act
        await purchaseTimeoutHandler.ExecuteAsync();

        // Assert
        asyncEventDispatcher.Verify(x => x.PublishAsync(It.IsAny<PurchaseTimeout>()), Times.Never);
    }

    private static IEnumerable<IPurchaseTimeoutPolicy> CreateMockPurchaseTimeoutPolicies(bool canApply)
    {
        var mockPurchaseTimeoutPolicy = new Mock<IPurchaseTimeoutPolicy>();
        mockPurchaseTimeoutPolicy.Setup(x => x.CanApply(It.IsAny<Purchase>())).Returns(canApply);

        var purchaseTimeoutPolicies = new List<IPurchaseTimeoutPolicy>
        {
            mockPurchaseTimeoutPolicy.Object
        };

        return purchaseTimeoutPolicies;
    }
}