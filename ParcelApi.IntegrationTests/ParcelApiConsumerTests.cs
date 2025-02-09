using AutoFixture.Xunit2;

using Core.Commands.Parcel;
using Core.Common.ErrorResponses;
using Core.Dto;
using Core.Queries.Parcel;

using FluentAssertions;

using MassTransit;
using MassTransit.Testing;

using ParcelApi.Data.Models;

using Test.Utility;


namespace ParcelApi.IntegrationTests;

[Collection(nameof(IntegrationTestsCollection))]
public class ParcelApiConsumerTests : IDisposable
{
    private readonly ParcelApiWebApplicationFactory _factory;

    public ParcelApiConsumerTests()
    {
        _factory = new();
        _factory.EnsureDbCreated();
        _factory.TestHarness.Start();
    }

    public void Dispose()
    {
        _factory.EnsureDbDeleted();
        _factory.Dispose();
    }


    [Theory, AutoData]
    public async Task CreateParcelRequest_ShouldCreateParcelAndReturnCreateParcelResponse(CreateParcelRequest request)
    {
        // Arrange
        Parcel expectedParcel = new()
        {
            Id = 1,
            Status = ParcelStatus.Pending,
            UserId = request.UserId,
            ParcelDetails = new() { Id = 1 },
            DeliveryDetails = new()
            {
                Id = 1,
                SourceAddress = request.SourceAddress,
                DestinationAddress = request.DestinationAddress,
            }
        };

        IRequestClient<CreateParcelRequest> client = _factory.TestHarness.GetRequestClient<CreateParcelRequest>();

        // Act
        Response<CreateParcelResponse> response = await client.GetResponse<CreateParcelResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, CreateParcelRequest, CreateParcelResponse>();


        response.Message.Parcel.Should().BeEquivalentTo(expectedParcel);
    }


    [Theory, AutoData]
    public async Task GetParcelRequest_ParcelExist_ShouldReturnGetParcelResponseWithParcel(
        GetParcelRequest request,
        Parcel expectedParcel)
    {
        // Arrange
        request.ParcelId = expectedParcel.Id;

        await _factory.SeedDataAsync([expectedParcel]);

        IRequestClient<GetParcelRequest> client = _factory.TestHarness.GetRequestClient<GetParcelRequest>();

        // Act
        Response<GetParcelResponse> response = await client.GetResponse<GetParcelResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, GetParcelRequest, GetParcelResponse>();

        response.Message.Parcel.Should().BeEquivalentTo(expectedParcel);
    }

    [Theory, AutoData]
    public async Task GetParcelRequest_ParcelNotExist_ShouldReturnGetParcelResponseWithNull(GetParcelRequest request)
    {
        // Arrange
        IRequestClient<GetParcelRequest> client = _factory.TestHarness.GetRequestClient<GetParcelRequest>();

        // Act
        Response<GetParcelResponse> response = await client.GetResponse<GetParcelResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, GetParcelRequest, GetParcelResponse>();

        response.Message.Parcel.Should().BeNull();
    }


    [Theory, AutoData]
    public async Task GetParcelsRequest_ParcelsExist_ShouldReturnGetParcelsResponseWithParcels(
        GetParcelsRequest request,
        List<Parcel> expectedParcels)
    {
        // Arrange
        await _factory.SeedDataAsync(expectedParcels);

        IRequestClient<GetParcelsRequest> client = _factory.TestHarness.GetRequestClient<GetParcelsRequest>();

        // Act
        Response<GetParcelsResponse> response = await client.GetResponse<GetParcelsResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, GetParcelsRequest, GetParcelsResponse>();

        response.Message.Parcels.Should().BeEquivalentTo(expectedParcels);
    }

    [Theory, AutoData]
    public async Task GetParcelsRequest_ParcelsNotExist_ShouldReturnGetParcelsResponseWithEmptyCollection(
        GetParcelsRequest request)
    {
        // Arrange
        IRequestClient<GetParcelsRequest> client = _factory.TestHarness.GetRequestClient<GetParcelsRequest>();

        // Act
        Response<GetParcelsResponse> response = await client.GetResponse<GetParcelsResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, GetParcelsRequest, GetParcelsResponse>();

        response.Message.Parcels.Should().BeEmpty();
    }


    [Theory, AutoData]
    public async Task GetParcelsByUserRequest_ParcelsExist_ShouldReturnGetParcelsByUserResponseWithUsersParcels(
        GetParcelsByUserRequest request,
        List<Parcel> randomParcels,
        List<Parcel> userParcels)
    {
        // Arrange
        userParcels.ForEach(x => x.UserId = request.UserId);

        await _factory.SeedDataAsync(randomParcels.Union(userParcels));

        IRequestClient<GetParcelsByUserRequest> client =
            _factory.TestHarness.GetRequestClient<GetParcelsByUserRequest>();

        // Act
        Response<GetParcelsByUserResponse> response = await client.GetResponse<GetParcelsByUserResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, GetParcelsByUserRequest, GetParcelsByUserResponse>();

        response.Message.Parcels.Should().BeEquivalentTo(userParcels).And.NotBeEquivalentTo(randomParcels);
    }

    [Theory, AutoData]
    public async Task
        GetParcelsByUserRequest_ParcelsNotExist_ShouldReturnGetParcelsByUserResponseWithWithEmptyCollection(
            GetParcelsByUserRequest request,
            List<Parcel> randomParcels)
    {
        // Arrange
        await _factory.SeedDataAsync(randomParcels);

        IRequestClient<GetParcelsByUserRequest> client =
            _factory.TestHarness.GetRequestClient<GetParcelsByUserRequest>();

        // Act
        Response<GetParcelsByUserResponse> response = await client.GetResponse<GetParcelsByUserResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, GetParcelsByUserRequest, GetParcelsByUserResponse>();

        response.Message.Parcels.Should().BeEmpty();
    }


    [Theory, AutoData]
    public async Task
        GetParcelsByCourierRequest_ParcelsExist_ShouldReturnGetParcelsByCourierResponseWithUsersParcels(
            GetParcelsByCourierRequest request,
            List<Parcel> randomParcels,
            List<Parcel> courierParcels)
    {
        // Arrange
        courierParcels.ForEach(x => x.CourierId = request.CourierId);

        await _factory.SeedDataAsync(randomParcels.Union(courierParcels));

        IRequestClient<GetParcelsByCourierRequest> client =
            _factory.TestHarness.GetRequestClient<GetParcelsByCourierRequest>();

        // Act
        Response<GetParcelsByCourierResponse> response = await client.GetResponse<GetParcelsByCourierResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, GetParcelsByCourierRequest, GetParcelsByCourierResponse>();

        response.Message.Parcels.Should().BeEquivalentTo(courierParcels).And.NotBeEquivalentTo(randomParcels);
    }

    [Theory, AutoData]
    public async Task
        GetParcelsByCourierRequest_ParcelsNotExist_ShouldReturnGetParcelsByCourierResponseWithWithEmptyCollection(
            GetParcelsByCourierRequest request,
            List<Parcel> randomParcels)
    {
        // Arrange
        await _factory.SeedDataAsync(randomParcels);

        IRequestClient<GetParcelsByCourierRequest> client =
            _factory.TestHarness.GetRequestClient<GetParcelsByCourierRequest>();

        // Act
        Response<GetParcelsByCourierResponse> response = await client.GetResponse<GetParcelsByCourierResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, GetParcelsByCourierRequest, GetParcelsByCourierResponse>();

        response.Message.Parcels.Should().BeEmpty();
    }


    [Theory, AutoData]
    public async Task UpdateDestinationRequest_ParcelNotExist_ShouldReturnParcelNotFoundResponse(
        UpdateDestinationRequest request,
        List<Parcel> randomParcels)
    {
        // Arrange
        await _factory.SeedDataAsync(randomParcels);

        IRequestClient<UpdateDestinationRequest> client =
            _factory.TestHarness.GetRequestClient<UpdateDestinationRequest>();

        // Act
        Response<UpdateDestinationResponse, ParcelNotFoundResponse> response =
            await client.GetResponse<UpdateDestinationResponse, ParcelNotFoundResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, UpdateDestinationRequest, ParcelNotFoundResponse>();

        response.Message
            .Should()
            .BeOfType<ParcelNotFoundResponse>()
            .And.Subject.As<ParcelNotFoundResponse>()
            .Should()
            .BeEquivalentTo(new ParcelNotFoundResponse
            {
                ParcelId = request.ParcelId
            });
    }

    [Theory, AutoData]
    public async Task UpdateDestinationRequest_PendingParcelExist_ShouldReturnUpdateDestinationResponseAndUpdateParcel(
        UpdateDestinationRequest request,
        List<Parcel> randomParcels)
    {
        // Arrange
        Parcel parcel = randomParcels.First();

        string oldDestinationAddress = parcel.DeliveryDetails.DestinationAddress;

        parcel.Id = request.ParcelId;
        parcel.Status = ParcelStatus.Pending;

        await _factory.SeedDataAsync(randomParcels);

        IRequestClient<UpdateDestinationRequest> client =
            _factory.TestHarness.GetRequestClient<UpdateDestinationRequest>();

        // Act
        Response<UpdateDestinationResponse> response = await client.GetResponse<UpdateDestinationResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, UpdateDestinationRequest, UpdateDestinationResponse>();

        response.Message.Updated.Should().BeTrue();

        response.Message.Parcel.DeliveryDetails.DestinationAddress
            .Should()
            .Be(request.NewDestination)
            .And.NotBeEquivalentTo(oldDestinationAddress);
    }

    [Theory]
    [InlineAutoData(ParcelStatus.InProgress)]
    [InlineAutoData(ParcelStatus.Delivered)]
    [InlineAutoData(ParcelStatus.Canceled)]
    [InlineAutoData(ParcelStatus.Assigned)]
    [InlineAutoData(ParcelStatus.None)]
    public async Task
        UpdateDestinationRequest_NonPendingParcelExist_ShouldReturnUpdateDestinationResponseAndNotUpdateParcel(
            ParcelStatus status,
            UpdateDestinationRequest request,
            List<Parcel> randomParcels)
    {
        // Arrange
        Parcel parcel = randomParcels.First();

        string oldDestinationAddress = parcel.DeliveryDetails.DestinationAddress;

        parcel.Id = request.ParcelId;
        parcel.Status = status;

        await _factory.SeedDataAsync(randomParcels);

        IRequestClient<UpdateDestinationRequest> client =
            _factory.TestHarness.GetRequestClient<UpdateDestinationRequest>();

        // Act
        Response<UpdateDestinationResponse> response = await client.GetResponse<UpdateDestinationResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, UpdateDestinationRequest, UpdateDestinationResponse>();

        response.Message.Updated.Should().BeFalse();

        response.Message.Parcel.DeliveryDetails.DestinationAddress
            .Should()
            .Be(oldDestinationAddress)
            .And.NotBeEquivalentTo(request.NewDestination);
    }


    [Theory, AutoData]
    public async Task UpdateParcelStatusRequest_ParcelNotExist_ShouldReturnParcelNotFoundResponse(
        UpdateParcelStatusRequest request,
        List<Parcel> randomParcels)
    {
        // Arrange
        await _factory.SeedDataAsync(randomParcels);

        IRequestClient<UpdateParcelStatusRequest> client =
            _factory.TestHarness.GetRequestClient<UpdateParcelStatusRequest>();

        // Act
        Response<UpdateParcelStatusResponse, ParcelNotFoundResponse> response =
            await client.GetResponse<UpdateParcelStatusResponse, ParcelNotFoundResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, UpdateParcelStatusRequest, ParcelNotFoundResponse>();

        response.Message
            .Should()
            .BeOfType<ParcelNotFoundResponse>()
            .And.Subject.As<ParcelNotFoundResponse>()
            .Should()
            .BeEquivalentTo(new ParcelNotFoundResponse
            {
                ParcelId = request.ParcelId
            });
    }

    [Theory, AutoData]
    public async Task UpdateParcelStatusRequest_ParcelExist_ShouldReturnUpdateParcelStatusResponseAndUpdateParcel(
        UpdateParcelStatusRequest request,
        List<Parcel> randomParcels)
    {
        // Arrange
        Parcel parcel = randomParcels.First();

        ParcelStatus oldStatus = parcel.Status;

        parcel.Id = request.ParcelId;

        await _factory.SeedDataAsync(randomParcels);

        IRequestClient<UpdateParcelStatusRequest> client =
            _factory.TestHarness.GetRequestClient<UpdateParcelStatusRequest>();

        // Act
        Response<UpdateParcelStatusResponse> response = await client.GetResponse<UpdateParcelStatusResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, UpdateParcelStatusRequest, UpdateParcelStatusResponse>();

        response.Message.Parcel.Status.Should().Be(request.NewStatus).And.NotBe(oldStatus);
    }


    [Theory, AutoData]
    public async Task AssignParcelRequest_ParcelNotExist_ShouldReturnParcelNotFoundResponse(
        AssignParcelRequest request,
        List<Parcel> randomParcels)
    {
        // Arrange
        await _factory.SeedDataAsync(randomParcels);

        IRequestClient<AssignParcelRequest> client = _factory.TestHarness.GetRequestClient<AssignParcelRequest>();

        // Act
        Response<AssignParcelRequest, ParcelNotFoundResponse> response =
            await client.GetResponse<AssignParcelRequest, ParcelNotFoundResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, AssignParcelRequest, ParcelNotFoundResponse>();

        response.Message
            .Should()
            .BeOfType<ParcelNotFoundResponse>()
            .And.Subject.As<ParcelNotFoundResponse>()
            .Should()
            .BeEquivalentTo(new ParcelNotFoundResponse
            {
                ParcelId = request.ParcelId
            });
    }

    [Theory, AutoData]
    public async Task AssignParcelRequest_ParcelExist_ShouldReturnAssignParcelResponseAndUpdateParcel(
        AssignParcelRequest request,
        List<Parcel> randomParcels)
    {
        // Arrange
        Parcel parcel = randomParcels.First();

        int? oldCourierId = parcel.CourierId;

        parcel.Id = request.ParcelId;

        await _factory.SeedDataAsync(randomParcels);

        IRequestClient<AssignParcelRequest> client = _factory.TestHarness.GetRequestClient<AssignParcelRequest>();

        // Act
        Response<AssignParcelResponse> response = await client.GetResponse<AssignParcelResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, AssignParcelRequest, AssignParcelResponse>();

        response.Message.Parcel.CourierId.Should().Be(request.CourierId).And.NotBe(oldCourierId);
    }


    [Theory, AutoData]
    public async Task CancelParcelRequest_ParcelNotExist_ShouldReturnParcelNotFoundResponse(
        CancelParcelRequest request,
        List<Parcel> randomParcels)
    {
        // Arrange
        await _factory.SeedDataAsync(randomParcels);

        IRequestClient<CancelParcelRequest> client = _factory.TestHarness.GetRequestClient<CancelParcelRequest>();

        // Act
        Response<CancelParcelRequest, ParcelNotFoundResponse> response =
            await client.GetResponse<CancelParcelRequest, ParcelNotFoundResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, CancelParcelRequest, ParcelNotFoundResponse>();

        response.Message
            .Should()
            .BeOfType<ParcelNotFoundResponse>()
            .And.Subject.As<ParcelNotFoundResponse>()
            .Should()
            .BeEquivalentTo(new ParcelNotFoundResponse
            {
                ParcelId = request.ParcelId
            });
    }

    [Theory]
    [InlineAutoData(ParcelStatus.Canceled)]
    [InlineAutoData(ParcelStatus.Pending)]
    [InlineAutoData(ParcelStatus.None)]
    public async Task CancelParcelRequest_ParcelExist_ShouldReturnCancelParcelResponseAndCancelParcel(
        ParcelStatus status,
        CancelParcelRequest request,
        List<Parcel> randomParcels)
    {
        // Arrange
        Parcel parcel = randomParcels.First();

        parcel.Id = request.ParcelId;
        parcel.Status = status;

        await _factory.SeedDataAsync(randomParcels);

        IRequestClient<CancelParcelRequest> client = _factory.TestHarness.GetRequestClient<CancelParcelRequest>();

        // Act
        Response<CancelParcelResponse> response = await client.GetResponse<CancelParcelResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, CancelParcelRequest, CancelParcelResponse>();

        response.Message.Parcel.Status.Should().Be(ParcelStatus.Canceled);
        response.Message.Canceled.Should().BeTrue();
    }

    [Theory]
    [InlineAutoData(ParcelStatus.InProgress)]
    [InlineAutoData(ParcelStatus.Delivered)]
    [InlineAutoData(ParcelStatus.Assigned)]
    public async Task CancelParcelRequest_ParcelExist_ShouldReturnCancelParcelResponseAndNotCancelParcel(
        ParcelStatus status,
        CancelParcelRequest request,
        List<Parcel> randomParcels)
    {
        // Arrange
        Parcel parcel = randomParcels.First();

        parcel.Id = request.ParcelId;
        parcel.Status = status;

        await _factory.SeedDataAsync(randomParcels);

        IRequestClient<CancelParcelRequest> client = _factory.TestHarness.GetRequestClient<CancelParcelRequest>();

        // Act
        Response<CancelParcelResponse> response = await client.GetResponse<CancelParcelResponse>(request);

        // Assert
        await _factory.AssertResponse<Services.ParcelApi, CancelParcelRequest, CancelParcelResponse>();

        response.Message.Parcel.Status.Should().Be(status);
        response.Message.Canceled.Should().BeFalse();
    }
}