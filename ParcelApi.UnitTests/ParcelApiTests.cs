using AutoFixture.Xunit2;

using AutoMapper;

using Core.Commands.Parcel;
using Core.Dto;
using Core.Queries.Parcel;

using FluentAssertions;
using FluentAssertions.ArgumentMatchers.Moq;

using MassTransit;

using Moq;

using ParcelApi.Data.Abstraction;
using ParcelApi.Data.Models;

using Test.Utility;


namespace ParcelApi.UnitTests;

public class ParcelApiTests
{
    [Theory, InlineAutoMoqData(typeof(ParcelApiCustomization))]
    public async Task CreateParcel_ShouldCreateParcel(
        ConsumeContext<CreateParcelRequest> context,
        [Frozen] Mock<IParcelDbContext> dbMock,
        Services.ParcelApi sut)
    {
        // Arrange
        Parcel parcel = new()
        {
            Status = ParcelStatus.Pending,
            UserId = context.Message.UserId,
            ParcelDetails = new(),
            DeliveryDetails = new()
            {
                SourceAddress = context.Message.SourceAddress,
                DestinationAddress = context.Message.DestinationAddress,
            }
        };

        dbMock.Setup(db => db.AddParcelAsync(Its.EquivalentTo(parcel))).Verifiable();

        // Act
        await sut.Consume(context);

        // Assert
        dbMock.Verify();
        dbMock.VerifyNoOtherCalls();
    }


    [Theory, InlineAutoMoqData(typeof(ParcelApiCustomization))]
    public async Task GetParcel_ParcelFound_ShouldReturnParcel(
        Parcel parcel,
        ConsumeContext<GetParcelRequest> context,
        [Frozen] Mock<IParcelDbContext> dbMock,
        Services.ParcelApi sut)
    {
        // Arrange
        dbMock.Setup(db => db.GetParcelByIdAsync(context.Message.ParcelId)).ReturnsAsync(parcel).Verifiable();

        // Act
        await sut.Consume(context);

        // Assert
        dbMock.Verify();
        dbMock.VerifyNoOtherCalls();
    }

    [Theory, InlineAutoMoqData(typeof(ParcelApiCustomization))]
    public async Task GetParcel_ParcelNotfound_ShouldNotFail(
        ConsumeContext<GetParcelRequest> context,
        [Frozen] Mock<IParcelDbContext> dbMock,
        Services.ParcelApi sut)
    {
        // Arrange
        dbMock.Setup(db => db.GetParcelByIdAsync(context.Message.ParcelId)).ReturnsAsync((Parcel)null).Verifiable();

        // Act & Assert
        await sut.Invoking(s => s.Consume(context)).Should().NotThrowAsync();

        dbMock.Verify();
        dbMock.VerifyNoOtherCalls();
    }


    [Theory, InlineAutoMoqData(typeof(ParcelApiCustomization))]
    public async Task GetParcels_ParcelsFound_ShouldReturnAllParcels(
        List<Parcel> parcels,
        ConsumeContext<GetParcelsRequest> context,
        [Frozen] Mock<IParcelDbContext> dbMock,
        Services.ParcelApi sut)
    {
        // Arrange
        dbMock.Setup(db => db.GetAllParcelsAsync()).ReturnsAsync(parcels).Verifiable();

        // Act
        await sut.Consume(context);

        // Assert
        dbMock.Verify();
        dbMock.VerifyNoOtherCalls();
    }

    [Theory, InlineAutoMoqData(typeof(ParcelApiCustomization))]
    public async Task GetParcels_ParcelsNotFound_ShouldNotFail(
        ConsumeContext<GetParcelsRequest> context,
        [Frozen] Mock<IParcelDbContext> dbMock,
        Services.ParcelApi sut)
    {
        // Arrange
        dbMock.Setup(db => db.GetAllParcelsAsync()).ReturnsAsync((List<Parcel>)null).Verifiable();

        // Act & Assert
        await sut.Invoking(s => s.Consume(context)).Should().NotThrowAsync();

        dbMock.Verify();
        dbMock.VerifyNoOtherCalls();
    }


    [Theory, InlineAutoMoqData(typeof(ParcelApiCustomization))]
    public async Task GetParcelsByUser_ParcelsExist_ShouldReturnParcels(
        List<Parcel> parcels,
        ConsumeContext<GetParcelsByUserRequest> context,
        [Frozen] Mock<IParcelDbContext> dbMock,
        [Frozen] Mock<IMapper> mapperMock,
        Services.ParcelApi sut)
    {
        // Arrange
        var randomParcels = parcels.Select(x => x.Clone()).Cast<Parcel>().ToList();

        parcels.ForEach(p => p.UserId = context.Message.UserId);

        var allParcels = randomParcels.Union(parcels).ToList();

        dbMock.Setup(db => db.GetAllParcelsAsync()).ReturnsAsync(allParcels).Verifiable();

        mapperMock.Setup(m => m.Map<IEnumerable<ParcelDto>>(Its.EquivalentTo(parcels))).Verifiable();

        // Act
        await sut.Consume(context);

        // Assert
        dbMock.Verify();
        mapperMock.Verify();
    }

    [Theory, InlineAutoMoqData(typeof(ParcelApiCustomization))]
    public async Task GetParcelsByUser_ParcelsNotExist_ShouldNotFail(
        List<Parcel> parcels,
        ConsumeContext<GetParcelsByUserRequest> context,
        [Frozen] Mock<IParcelDbContext> dbMock,
        [Frozen] Mock<IMapper> mapperMock,
        Services.ParcelApi sut)
    {
        // Arrange
        dbMock.Setup(db => db.GetAllParcelsAsync()).ReturnsAsync(parcels).Verifiable();

        mapperMock.Setup(m => m.Map<IEnumerable<ParcelDto>>(Its.EquivalentTo(Enumerable.Empty<Parcel>()))).Verifiable();

        // Act & Assert
        await sut.Invoking(s => s.Consume(context)).Should().NotThrowAsync();

        dbMock.Verify();
        mapperMock.Verify();
    }


    [Theory, InlineAutoMoqData(typeof(ParcelApiCustomization))]
    public async Task GetParcelsByCourier_ParcelsExist_ShouldReturnParcels(
        List<Parcel> parcels,
        ConsumeContext<GetParcelsByCourierRequest> context,
        [Frozen] Mock<IParcelDbContext> dbMock,
        [Frozen] Mock<IMapper> mapperMock,
        Services.ParcelApi sut)
    {
        // Arrange
        var randomParcels = parcels.Select(x => x.Clone()).Cast<Parcel>().ToList();

        parcels.ForEach(p => p.CourierId = context.Message.CourierId);

        var allParcels = randomParcels.Union(parcels).ToList();

        dbMock.Setup(db => db.GetAllParcelsAsync()).ReturnsAsync(allParcels).Verifiable();

        mapperMock.Setup(m => m.Map<IEnumerable<ParcelDto>>(Its.EquivalentTo(parcels))).Verifiable();

        // Act
        await sut.Consume(context);

        // Assert
        dbMock.Verify();
        mapperMock.Verify();
    }

    [Theory, InlineAutoMoqData(typeof(ParcelApiCustomization))]
    public async Task GetParcelsByCourier_ParcelsNotExist_ShouldNotFail(
        List<Parcel> parcels,
        ConsumeContext<GetParcelsByCourierRequest> context,
        [Frozen] Mock<IParcelDbContext> dbMock,
        [Frozen] Mock<IMapper> mapperMock,
        Services.ParcelApi sut)
    {
        // Arrange
        dbMock.Setup(db => db.GetAllParcelsAsync()).ReturnsAsync(parcels).Verifiable();

        mapperMock.Setup(m => m.Map<IEnumerable<ParcelDto>>(Its.EquivalentTo(Enumerable.Empty<Parcel>()))).Verifiable();

        // Act & Assert
        await sut.Invoking(s => s.Consume(context)).Should().NotThrowAsync();

        dbMock.Verify();
        mapperMock.Verify();
    }


    [Theory]
    [InlineAutoMoqData(typeof(ParcelApiCustomization), ParcelStatus.Delivered)]
    [InlineAutoMoqData(typeof(ParcelApiCustomization), ParcelStatus.Assigned)]
    [InlineAutoMoqData(typeof(ParcelApiCustomization), ParcelStatus.Canceled)]
    [InlineAutoMoqData(typeof(ParcelApiCustomization), ParcelStatus.InProgress)]
    [InlineAutoMoqData(typeof(ParcelApiCustomization), ParcelStatus.None)]
    public async Task UpdateDestination_StatusIsNotPending_ShouldNotUpdateDestination(
        ParcelStatus status,
        Parcel parcel,
        [Frozen] Mock<IParcelDbContext> dbMock,
        ConsumeContext<UpdateDestinationRequest> context,
        Services.ParcelApi sut)
    {
        // Arrange
        string oldDestinationAddress = parcel.DeliveryDetails.DestinationAddress;
        parcel.Status = status;

        dbMock.Setup(db => db.GetParcelByIdAsync(context.Message.ParcelId)).ReturnsAsync(parcel).Verifiable();

        // Act
        await sut.Consume(context);

        // Assert
        parcel.DeliveryDetails.DestinationAddress
            .Should()
            .Be(oldDestinationAddress)
            .And.NotBe(context.Message.NewDestination);

        dbMock.Verify();
        dbMock.VerifyNoOtherCalls();
    }

    [Theory, InlineAutoMoqData(typeof(ParcelApiCustomization))]
    public async Task UpdateDestination_StatusIsPending_ShouldUpdateDestination(
        Parcel parcel,
        [Frozen] Mock<IParcelDbContext> dbMock,
        ConsumeContext<UpdateDestinationRequest> context,
        Services.ParcelApi sut)
    {
        // Arrange
        string oldDestinationAddress = parcel.DeliveryDetails.DestinationAddress;
        parcel.Status = ParcelStatus.Pending;

        dbMock.Setup(db => db.GetParcelByIdAsync(context.Message.ParcelId)).ReturnsAsync(parcel).Verifiable();
        dbMock.Setup(db => db.UpdateParcelAsync(Its.EquivalentTo(parcel))).Verifiable();

        // Act
        await sut.Consume(context);

        // Assert
        parcel.DeliveryDetails.DestinationAddress
            .Should()
            .Be(context.Message.NewDestination)
            .And.NotBe(oldDestinationAddress);

        dbMock.Verify();
        dbMock.VerifyNoOtherCalls();
    }


    [Theory, InlineAutoMoqData(typeof(ParcelApiCustomization))]
    public async Task UpdateDestination_ParcelNotFound_ShouldNotFail(
        [Frozen] Mock<IParcelDbContext> dbMock,
        ConsumeContext<UpdateDestinationRequest> context,
        Services.ParcelApi sut)
    {
        // Arrange
        dbMock.Setup(db => db.GetParcelByIdAsync(context.Message.ParcelId)).ReturnsAsync((Parcel)null).Verifiable();

        // Act & Assert
        await sut.Invoking(s => s.Consume(context)).Should().NotThrowAsync();

        dbMock.Verify();
        dbMock.VerifyNoOtherCalls();
    }


    [Theory, InlineAutoMoqData(typeof(ParcelApiCustomization))]
    public async Task UpdateParcelStatus_ParcelFound_ShouldUpdateStatus(
        Parcel parcel,
        ConsumeContext<UpdateParcelStatusRequest> context,
        [Frozen] Mock<IParcelDbContext> dbMock,
        Services.ParcelApi sut)
    {
        // Arrange
        ParcelStatus oldStatus = parcel.Status;

        dbMock.Setup(db => db.GetParcelByIdAsync(context.Message.ParcelId)).ReturnsAsync(parcel).Verifiable();
        dbMock.Setup(db => db.UpdateParcelAsync(Its.EquivalentTo(parcel))).Verifiable();

        // Act
        await sut.Consume(context);

        // Assert
        parcel.Status.Should().Be(context.Message.NewStatus).And.NotBe(oldStatus);

        dbMock.Verify();
        dbMock.VerifyNoOtherCalls();
    }

    [Theory, InlineAutoMoqData(typeof(ParcelApiCustomization))]
    public async Task UpdateParcelStatus_ParcelNotFound_ShouldNotFail(
        ConsumeContext<UpdateParcelStatusRequest> context,
        [Frozen] Mock<IParcelDbContext> dbMock,
        Services.ParcelApi sut)
    {
        // Arrange
        dbMock.Setup(db => db.GetParcelByIdAsync(context.Message.ParcelId)).ReturnsAsync((Parcel)null).Verifiable();

        // Act
        await sut.Invoking(s => s.Consume(context)).Should().NotThrowAsync();

        // Assert
        dbMock.Verify();
        dbMock.VerifyNoOtherCalls();
    }


    [Theory, InlineAutoMoqData(typeof(ParcelApiCustomization))]
    public async Task AssignParcel_ParcelFound_ShouldAssignCourier(
        Parcel parcel,
        ConsumeContext<AssignParcelRequest> context,
        [Frozen] Mock<IParcelDbContext> dbMock,
        Services.ParcelApi sut)
    {
        // Arrange
        int? oldCourierId = parcel.CourierId;
        context.Message.ParcelId = parcel.Id;

        dbMock.Setup(db => db.GetParcelByIdAsync(parcel.Id)).ReturnsAsync(parcel).Verifiable();
        dbMock.Setup(db => db.UpdateParcelAsync(Its.EquivalentTo(parcel))).Verifiable();

        // Act
        await sut.Consume(context);

        // Assert
        parcel.CourierId.Should().Be(context.Message.CourierId).And.NotBe(oldCourierId);
        parcel.Status.Should().Be(ParcelStatus.Assigned);

        dbMock.Verify();
    }

    [Theory, InlineAutoMoqData(typeof(ParcelApiCustomization))]
    public async Task AssignParcel_ParcelNotFound_ShouldNotFail(
        Parcel parcel,
        ConsumeContext<AssignParcelRequest> context,
        [Frozen] Mock<IParcelDbContext> dbMock,
        Services.ParcelApi sut)
    {
        // Arrange
        int? oldCourierId = parcel.CourierId;

        dbMock.Setup(db => db.GetParcelByIdAsync(context.Message.ParcelId)).ReturnsAsync((Parcel)null).Verifiable();

        // Act & Assert
        await sut.Invoking(s => s.Consume(context)).Should().NotThrowAsync();

        parcel.CourierId.Should().Be(oldCourierId);

        dbMock.Verify();
        dbMock.VerifyNoOtherCalls();
    }


    [Theory]
    [InlineAutoMoqData(typeof(ParcelApiCustomization), ParcelStatus.Pending)]
    [InlineAutoMoqData(typeof(ParcelApiCustomization), ParcelStatus.None)]
    [InlineAutoMoqData(typeof(ParcelApiCustomization), ParcelStatus.Canceled)]
    public async Task CancelParcel_StatusAllowsToCancel_ShouldCancelParcel(
        ParcelStatus status,
        Parcel parcel,
        ConsumeContext<CancelParcelRequest> context,
        [Frozen] Mock<IParcelDbContext> dbMock,
        Services.ParcelApi sut)
    {
        // Arrange
        parcel.Status = status;

        dbMock.Setup(db => db.GetParcelByIdAsync(context.Message.ParcelId)).ReturnsAsync(parcel).Verifiable();
        dbMock.Setup(db => db.UpdateParcelAsync(Its.EquivalentTo(parcel))).Verifiable();

        // Act
        await sut.Consume(context);

        // Assert
        parcel.Status.Should().Be(ParcelStatus.Canceled);

        dbMock.Verify();
        dbMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineAutoMoqData(typeof(ParcelApiCustomization), ParcelStatus.Assigned)]
    [InlineAutoMoqData(typeof(ParcelApiCustomization), ParcelStatus.Delivered)]
    [InlineAutoMoqData(typeof(ParcelApiCustomization), ParcelStatus.InProgress)]
    public async Task CancelParcel_StatusCantBeCanceled_ShouldNotCancelParcel(
        ParcelStatus status,
        Parcel parcel,
        ConsumeContext<CancelParcelRequest> context,
        [Frozen] Mock<IParcelDbContext> dbMock,
        Services.ParcelApi sut)
    {
        // Arrange
        parcel.Status = status;

        dbMock.Setup(db => db.GetParcelByIdAsync(context.Message.ParcelId)).ReturnsAsync(parcel).Verifiable();

        // Act
        await sut.Consume(context);

        // Assert
        parcel.Status.Should().NotBe(ParcelStatus.Canceled);

        dbMock.Verify();
        dbMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineAutoMoqData(typeof(ParcelApiCustomization))]
    public async Task CancelParcel_ParcelNotFound_ShouldNotFail(
        ConsumeContext<CancelParcelRequest> context,
        [Frozen] Mock<IParcelDbContext> dbMock,
        Services.ParcelApi sut)
    {
        // Arrange
        dbMock.Setup(db => db.GetParcelByIdAsync(context.Message.ParcelId)).ReturnsAsync((Parcel)null).Verifiable();

        // Act & Assert
        await sut.Invoking(s => s.Consume(context)).Should().NotThrowAsync();

        dbMock.Verify();
        dbMock.VerifyNoOtherCalls();
    }
}