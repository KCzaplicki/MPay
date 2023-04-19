using AutoMapper;
using Moq;
using MPay.Abstractions.Common;
using MPay.Abstractions.Events;
using MPay.Core.DTO;
using MPay.Core.Entities;
using MPay.Core.Events;
using MPay.Core.Exceptions;
using MPay.Core.Factories;
using MPay.Core.Repository;
using MPay.Core.Services;
using MPay.Tests.Shared.Repositories;

namespace MPay.Core.Tests.Services;

public class PurchaseServiceTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IClock> _mockClock;
    private readonly Mock<IPurchaseFactory> _mockPurchaseFactory;

    public PurchaseServiceTests()
    {
        _mockClock = MockClockFactory.Create();
        _mapper = MapperFactory.Create();
        _mockPurchaseFactory = CreateMockPurchaseFactory(_mapper);
    }

    [Fact]
    public async Task AddAsync_AddPurchase()
    {
        // Arrange
        var addPurchaseDto = AutoFaker.Generate<AddPurchaseDto, AddPurchaseDtoFake>();
        var mockPurchaseRepository = new Mock<IPurchaseRepository>();
        var mockAsyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchaseService = new PurchaseService(mockPurchaseRepository.Object, _mockPurchaseFactory.Object,
            mockAsyncEventDispatcher.Object, _mapper, _mockClock.Object);

        // Act
        var purchaseId = await purchaseService.AddAsync(addPurchaseDto);

        // Assert
        purchaseId.Should().NotBeNullOrWhiteSpace();
        mockPurchaseRepository.Verify(x => x.AddAsync(It.IsAny<Purchase>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_PublishesPurchaseAddedEvent()
    {
        // Arrange
        var addPurchaseDto = AutoFaker.Generate<AddPurchaseDto, AddPurchaseDtoFake>();
        var mockPurchaseRepository = new Mock<IPurchaseRepository>();
        var mockAsyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchaseService = new PurchaseService(mockPurchaseRepository.Object, _mockPurchaseFactory.Object,
            mockAsyncEventDispatcher.Object, _mapper, _mockClock.Object);

        // Act
        var purchaseId = await purchaseService.AddAsync(addPurchaseDto);

        // Assert
        mockAsyncEventDispatcher.Verify(x => x.PublishAsync(It.Is<PurchaseCreated>(
            e => e.Id == purchaseId)), Times.Once);
    }

    [Fact]
    public async Task CancelPurchaseAsync_CancelsPurchase()
    {
        // Arrange
        var purchase = AutoFaker.Generate<Purchase, PendingPurchaseFake>();
        var mockPurchaseRepository = MockPurchaseRepositoryFactory.Create(purchase);
        var mockAsyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchaseService = new PurchaseService(mockPurchaseRepository.Object, _mockPurchaseFactory.Object,
            mockAsyncEventDispatcher.Object, _mapper, _mockClock.Object);

        // Act
        await purchaseService.CancelPurchaseAsync(purchase.Id);

        // Assert
        mockPurchaseRepository.Verify(x => x.UpdateAsync(It.Is<Purchase>(
            p => p.Id == purchase.Id && p.Status == PurchaseStatus.Cancelled)), Times.Once);
    }

    [Fact]
    public async Task CancelPurchaseAsync_PublishesPurchaseCancelledEvent()
    {
        // Arrange
        var purchase = AutoFaker.Generate<Purchase, PendingPurchaseFake>();
        var mockPurchaseRepository = MockPurchaseRepositoryFactory.Create(purchase);
        var mockAsyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchaseService = new PurchaseService(mockPurchaseRepository.Object, _mockPurchaseFactory.Object,
            mockAsyncEventDispatcher.Object, _mapper, _mockClock.Object);

        // Act
        await purchaseService.CancelPurchaseAsync(purchase.Id);

        // Assert
        mockAsyncEventDispatcher.Verify(x => x.PublishAsync(It.Is<PurchaseCancelled>(
            e => e.Id == purchase.Id)), Times.Once);
    }

    [Fact]
    public async Task CancelPurchaseAsync_ThrowsPurchaseNotFoundException_WhenPurchaseDoesNotExist()
    {
        // Arrange
        var mockPurchaseRepository = new Mock<IPurchaseRepository>();
        var mockAsyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchaseService = new PurchaseService(mockPurchaseRepository.Object, _mockPurchaseFactory.Object,
            mockAsyncEventDispatcher.Object, _mapper, _mockClock.Object);

        // Act
        Func<Task> act = async () => await purchaseService.CancelPurchaseAsync(Guid.NewGuid().ToString());

        // Assert
        await act.Should().ThrowAsync<PurchaseNotFoundException>();
    }

    [Fact]
    public async Task CancelPurchaseAsync_DoesNotUpdatePurchase_WhenPurchaseNotInPending()
    {
        // Arrange
        var purchase = AutoFaker.Generate<Purchase, CompletedPurchaseFake>();
        var mockPurchaseRepository = MockPurchaseRepositoryFactory.Create(purchase);
        var mockAsyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchaseService = new PurchaseService(mockPurchaseRepository.Object, _mockPurchaseFactory.Object,
            mockAsyncEventDispatcher.Object, _mapper, _mockClock.Object);

        // Act
        await purchaseService.CancelPurchaseAsync(purchase.Id);

        // Assert
        mockPurchaseRepository.Verify(x => x.UpdateAsync(It.IsAny<Purchase>()), Times.Never);
    }

    [Fact]
    public async Task GetPendingAsync_ReturnsPendingPurchase()
    {
        // Arrange
        var purchase = AutoFaker.Generate<Purchase, PendingPurchaseFake>();
        var mockPurchaseRepository = MockPurchaseRepositoryFactory.Create(purchase);
        var mockAsyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchaseService = new PurchaseService(mockPurchaseRepository.Object, _mockPurchaseFactory.Object,
            mockAsyncEventDispatcher.Object, _mapper, _mockClock.Object);

        // Act
        await purchaseService.GetPendingAsync(purchase.Id);

        // Assert
        mockPurchaseRepository.Verify(x => x.GetAsync(It.Is<string>(
            p => p == purchase.Id)), Times.Once);
    }

    [Fact]
    public async Task GetPendingAsync_ThrowsPurchaseNotFoundException_WhenPurchaseDoesNotExist()
    {
        // Arrange
        var mockPurchaseRepository = new Mock<IPurchaseRepository>();
        var mockAsyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchaseService = new PurchaseService(mockPurchaseRepository.Object, _mockPurchaseFactory.Object,
            mockAsyncEventDispatcher.Object, _mapper, _mockClock.Object);

        // Act
        Func<Task> act = async () => await purchaseService.GetPendingAsync(Guid.NewGuid().ToString());

        // Assert
        await act.Should().ThrowAsync<PurchaseNotFoundException>();
    }

    [Fact]
    public async Task GetPendingAsync_ThrowsPurchaseNotPendingException_WhenPurchaseNotPending()
    {
        // Arrange
        var purchase = AutoFaker.Generate<Purchase, CompletedPurchaseFake>();
        var mockPurchaseRepository = MockPurchaseRepositoryFactory.Create(purchase);
        var mockAsyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchaseService = new PurchaseService(mockPurchaseRepository.Object, _mockPurchaseFactory.Object,
            mockAsyncEventDispatcher.Object, _mapper, _mockClock.Object);

        // Act
        Func<Task> act = async () => await purchaseService.GetPendingAsync(purchase.Id);

        // Assert
        await act.Should().ThrowAsync<PurchaseStatusNotPendingException>();
    }

    private static Mock<IPurchaseFactory> CreateMockPurchaseFactory(IMapper mapper)
    {
        var mockPurchaseFactory = new Mock<IPurchaseFactory>();
        mockPurchaseFactory.Setup(x => x.Create(It.IsAny<AddPurchaseDto>()))
            .Returns((AddPurchaseDto dto) =>
            {
                var purchase = mapper.Map<Purchase>(dto);
                purchase.Id = Guid.NewGuid().ToString();

                return purchase;
            });

        return mockPurchaseFactory;
    }
}