using MPay.Core.Entities;
using MPay.Core.Repository;

namespace MPay.Tests.Shared.Repositories;

public static class MockPurchaseRepositoryFactory
{
    public static Mock<IPurchaseRepository> Create(List<Purchase> purchases)
    {
        var mockPurchaseRepository = new Mock<IPurchaseRepository>();
        mockPurchaseRepository.Setup(x => x.GetPendingAsync()).ReturnsAsync(purchases);
        mockPurchaseRepository.Setup(x => x.GetAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) => purchases.FirstOrDefault(p => p.Id == id));
        mockPurchaseRepository.Setup(x => x.UpdateAsync(It.IsAny<Purchase>())).Returns(Task.CompletedTask);
        
        return mockPurchaseRepository;
    }
    
    public static Mock<IPurchaseRepository> Create(Purchase purchases) 
        => Create(new List<Purchase> { purchases });
}