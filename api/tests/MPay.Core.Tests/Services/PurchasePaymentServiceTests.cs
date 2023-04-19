using AutoMapper;
using Moq;
using MPay.Abstractions.Events;
using MPay.Core.DTO;
using MPay.Core.Entities;
using MPay.Core.Events;
using MPay.Core.Exceptions;
using MPay.Core.Factories;
using MPay.Core.Policies.PurchasePaymentStatus;
using MPay.Core.Repository;
using MPay.Core.Services;
using MPay.Tests.Shared.Repositories;

namespace MPay.Core.Tests.Services;

public class PurchasePaymentServiceTests
{
    private readonly Mock<IPurchasePaymentFactory> _mockPurchasePaymentFactory;

    public PurchasePaymentServiceTests()
    {
        var mapper = MapperFactory.Create();
        _mockPurchasePaymentFactory = CreateMockPurchasePaymentFactory(mapper);
    }

    [Fact]
    public async Task ProcessPaymentAsync_AddPurchasePayment()
    {
        // Arrange
        var purchasePaymentDto = AutoFaker.Generate<PurchasePaymentDto, PurchasePaymentDtoFake>();
        var purchase = AutoFaker.Generate<Purchase, PendingPurchaseFake>();
        var mockPurchaseRepository = MockPurchaseRepositoryFactory.Create(purchase);
        var mockPurchasePaymentRepository = new Mock<IPurchasePaymentRepository>();
        var mockPurchasePaymentStatusPolicies = CreateMockPurchasePaymentStatusPolices(PurchasePaymentStatus.Completed);
        var mockAsyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchasePaymentService = new PurchasePaymentService(mockPurchaseRepository.Object,
            mockPurchasePaymentRepository.Object,
            mockPurchasePaymentStatusPolicies, _mockPurchasePaymentFactory.Object, mockAsyncEventDispatcher.Object);

        // Act
        var purchasePaymentResultDto =
            await purchasePaymentService.ProcessPaymentAsync(purchase.Id, purchasePaymentDto);

        // Assert
        purchasePaymentResultDto.Should().NotBeNull();
        purchasePaymentResultDto.PurchasePaymentId.Should().NotBeNullOrEmpty();
        purchasePaymentResultDto.PurchaseId.Should().Be(purchase.Id);
        purchasePaymentResultDto.PurchasePaymentStatus.Should().Be(PurchasePaymentStatus.Completed);
        mockPurchasePaymentRepository.Verify(x => x.AddAsync(It.IsAny<PurchasePayment>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPaymentAsync_SetPurchaseStatusToCompleted_WhenPurchasePaymentStatusIsCompleted()
    {
        // Arrange
        var purchasePaymentDto = AutoFaker.Generate<PurchasePaymentDto, PurchasePaymentDtoFake>();
        var purchase = AutoFaker.Generate<Purchase, PendingPurchaseFake>();
        var mockPurchaseRepository = MockPurchaseRepositoryFactory.Create(purchase);
        var mockPurchasePaymentRepository = new Mock<IPurchasePaymentRepository>();
        var mockPurchasePaymentStatusPolicies = CreateMockPurchasePaymentStatusPolices(PurchasePaymentStatus.Completed);
        var mockAsyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchasePaymentService = new PurchasePaymentService(mockPurchaseRepository.Object,
            mockPurchasePaymentRepository.Object,
            mockPurchasePaymentStatusPolicies, _mockPurchasePaymentFactory.Object, mockAsyncEventDispatcher.Object);

        // Act
        await purchasePaymentService.ProcessPaymentAsync(purchase.Id, purchasePaymentDto);

        // Assert
        purchase.Status.Should().Be(PurchaseStatus.Completed);
        purchase.CompletedAt.Should().NotBeNull();
        mockPurchaseRepository.Verify(x => x.UpdateAsync(It.IsAny<Purchase>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPaymentAsync_PublishesPurchasePaymentCompletedEvent_WhenPurchaseStatusIsCompleted()
    {
        // Arrange
        var purchasePaymentDto = AutoFaker.Generate<PurchasePaymentDto, PurchasePaymentDtoFake>();
        var purchase = AutoFaker.Generate<Purchase, PendingPurchaseFake>();
        var mockPurchaseRepository = MockPurchaseRepositoryFactory.Create(purchase);
        var mockPurchasePaymentRepository = new Mock<IPurchasePaymentRepository>();
        var mockPurchasePaymentStatusPolicies = CreateMockPurchasePaymentStatusPolices(PurchasePaymentStatus.Completed);
        var mockAsyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchasePaymentService = new PurchasePaymentService(mockPurchaseRepository.Object,
            mockPurchasePaymentRepository.Object,
            mockPurchasePaymentStatusPolicies, _mockPurchasePaymentFactory.Object, mockAsyncEventDispatcher.Object);

        // Act
        await purchasePaymentService.ProcessPaymentAsync(purchase.Id, purchasePaymentDto);

        // Assert
        mockAsyncEventDispatcher.Verify(x => x.PublishAsync(It.Is<PurchaseCompleted>(
            e => e.Id == purchase.Id)), Times.Once);
    }

    [Fact]
    public async Task ProcessPaymentAsync_DoesNotUpdatePurchase_WhenPurchasePaymentStatusIsNotCompleted()
    {
        // Arrange
        var purchasePaymentDto = AutoFaker.Generate<PurchasePaymentDto, PurchasePaymentDtoFake>();
        var purchase = AutoFaker.Generate<Purchase, PendingPurchaseFake>();
        var mockPurchaseRepository = MockPurchaseRepositoryFactory.Create(purchase);
        var mockPurchasePaymentRepository = new Mock<IPurchasePaymentRepository>();
        var mockPurchasePaymentStatusPolicies = CreateMockPurchasePaymentStatusPolices(PurchasePaymentStatus.NoFounds);
        var mockAsyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchasePaymentService = new PurchasePaymentService(mockPurchaseRepository.Object,
            mockPurchasePaymentRepository.Object,
            mockPurchasePaymentStatusPolicies, _mockPurchasePaymentFactory.Object, mockAsyncEventDispatcher.Object);

        // Act
        await purchasePaymentService.ProcessPaymentAsync(purchase.Id, purchasePaymentDto);

        // Assert
        purchase.Status.Should().Be(PurchaseStatus.Pending);
        purchase.CompletedAt.Should().BeNull();
        mockPurchaseRepository.Verify(x => x.UpdateAsync(It.IsAny<Purchase>()), Times.Never);
    }

    [Fact]
    public async Task
        ProcessPaymentAsync_DoesNotPublishesPurchasePaymentCompletedEvent_WhenPurchaseStatusIsNotCompleted()
    {
        // Arrange
        var purchasePaymentDto = AutoFaker.Generate<PurchasePaymentDto, PurchasePaymentDtoFake>();
        var purchase = AutoFaker.Generate<Purchase, PendingPurchaseFake>();
        var mockPurchaseRepository = MockPurchaseRepositoryFactory.Create(purchase);
        var mockPurchasePaymentRepository = new Mock<IPurchasePaymentRepository>();
        var mockPurchasePaymentStatusPolicies = CreateMockPurchasePaymentStatusPolices(PurchasePaymentStatus.NoFounds);
        var mockAsyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchasePaymentService = new PurchasePaymentService(mockPurchaseRepository.Object,
            mockPurchasePaymentRepository.Object,
            mockPurchasePaymentStatusPolicies, _mockPurchasePaymentFactory.Object, mockAsyncEventDispatcher.Object);

        // Act
        await purchasePaymentService.ProcessPaymentAsync(purchase.Id, purchasePaymentDto);

        // Assert
        mockAsyncEventDispatcher.Verify(x => x.PublishAsync(It.Is<PurchaseCompleted>(
            e => e.Id == purchase.Id)), Times.Never);
    }

    [Fact]
    public async Task ProcessPaymentAsync_ThrowsPurchaseNotFoundException_WhenPurchaseNotExists()
    {
        // Arrange
        var purchasePaymentDto = AutoFaker.Generate<PurchasePaymentDto, PurchasePaymentDtoFake>();
        var purchaseId = Guid.NewGuid().ToString();
        var mockPurchaseRepository = MockPurchaseRepositoryFactory.Create(new List<Purchase>());
        var mockPurchasePaymentRepository = new Mock<IPurchasePaymentRepository>();
        var mockPurchasePaymentStatusPolicies = CreateMockPurchasePaymentStatusPolices(PurchasePaymentStatus.Completed);
        var mockAsyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchasePaymentService = new PurchasePaymentService(mockPurchaseRepository.Object,
            mockPurchasePaymentRepository.Object,
            mockPurchasePaymentStatusPolicies, _mockPurchasePaymentFactory.Object, mockAsyncEventDispatcher.Object);

        // Act
        Func<Task> act = async () => await purchasePaymentService.ProcessPaymentAsync(purchaseId, purchasePaymentDto);

        // Assert
        await act.Should().ThrowAsync<PurchaseNotFoundException>();
    }

    [Fact]
    public async Task ProcessPaymentAsync_ThrowsPurchaseStatusNotPendingException_WhenPurchaseStatusIsNotPending()
    {
        // Arrange
        var purchasePaymentDto = AutoFaker.Generate<PurchasePaymentDto, PurchasePaymentDtoFake>();
        var purchase = AutoFaker.Generate<Purchase, CompletedPurchaseFake>();
        var mockPurchaseRepository = MockPurchaseRepositoryFactory.Create(purchase);
        var mockPurchasePaymentRepository = new Mock<IPurchasePaymentRepository>();
        var mockPurchasePaymentStatusPolicies = CreateMockPurchasePaymentStatusPolices(PurchasePaymentStatus.Completed);
        var mockAsyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchasePaymentService = new PurchasePaymentService(mockPurchaseRepository.Object,
            mockPurchasePaymentRepository.Object,
            mockPurchasePaymentStatusPolicies, _mockPurchasePaymentFactory.Object, mockAsyncEventDispatcher.Object);

        // Act
        Func<Task> act = async () => await purchasePaymentService.ProcessPaymentAsync(purchase.Id, purchasePaymentDto);

        // Assert
        await act.Should().ThrowAsync<PurchaseStatusNotPendingException>();
    }

    [Fact]
    public async Task
        ProcessPaymentAsync_ThrowsPurchasePaymentNotProcessedException_WhenPurchasePaymentStatusIsNotProcessed()
    {
        // Arrange
        var purchasePaymentDto = AutoFaker.Generate<PurchasePaymentDto, PurchasePaymentDtoFake>();
        var purchase = AutoFaker.Generate<Purchase, PendingPurchaseFake>();
        var mockPurchaseRepository = MockPurchaseRepositoryFactory.Create(purchase);
        var mockPurchasePaymentRepository = new Mock<IPurchasePaymentRepository>();
        var mockPurchasePaymentStatusPolicies = CreateMockPurchasePaymentStatusPolices(default, false);
        var mockAsyncEventDispatcher = new Mock<IAsyncEventDispatcher>();

        var purchasePaymentService = new PurchasePaymentService(mockPurchaseRepository.Object,
            mockPurchasePaymentRepository.Object,
            mockPurchasePaymentStatusPolicies, _mockPurchasePaymentFactory.Object, mockAsyncEventDispatcher.Object);

        // Act
        Func<Task> act = async () => await purchasePaymentService.ProcessPaymentAsync(purchase.Id, purchasePaymentDto);

        // Assert
        await act.Should().ThrowAsync<PurchasePaymentNotProcessedException>();
    }

    private static IEnumerable<IPurchasePaymentStatusPolicy> CreateMockPurchasePaymentStatusPolices(
        PurchasePaymentStatus status, bool canApply = true)
    {
        var mockPurchasePaymentStatusPolicy = new Mock<IPurchasePaymentStatusPolicy>();
        mockPurchasePaymentStatusPolicy.Setup(x =>
            x.CanApply(It.IsAny<PurchasePayment>())).Returns(canApply);
        mockPurchasePaymentStatusPolicy.Setup(x => x.Apply(It.IsAny<PurchasePayment>()))
            .Callback((PurchasePayment p) => p.Status = status);

        var mockPurchasePaymentStatusPolicies = new List<IPurchasePaymentStatusPolicy>
        {
            mockPurchasePaymentStatusPolicy.Object
        };

        return mockPurchasePaymentStatusPolicies;
    }


    private static Mock<IPurchasePaymentFactory> CreateMockPurchasePaymentFactory(IMapper mapper)
    {
        var mockPurchasePaymentFactory = new Mock<IPurchasePaymentFactory>();
        mockPurchasePaymentFactory.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<PurchasePaymentDto>()))
            .Returns((string purchaseId, PurchasePaymentDto dto) =>
            {
                var purchasePayment = mapper.Map<PurchasePayment>(dto);
                purchasePayment.Id = Guid.NewGuid().ToString();
                purchasePayment.PurchaseId = purchaseId;

                return purchasePayment;
            });

        return mockPurchasePaymentFactory;
    }
}