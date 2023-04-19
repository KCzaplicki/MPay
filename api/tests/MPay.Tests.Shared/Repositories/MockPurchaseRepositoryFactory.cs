using MPay.Core.Entities;
using MPay.Core.Repository;

namespace MPay.Tests.Shared.Repositories;

public static class MockPurchaseRepositoryFactory
{
    public static Mock<IPurchaseRepository> Create(List<Purchase> purchases)
    {
        var mockPurchaseRepository = new Mock<IPurchaseRepository>();
        mockPurchaseRepository.Setup(x => x.GetPendingAsync()).ReturnsAsync(purchases);
        
        return mockPurchaseRepository;
    }
}