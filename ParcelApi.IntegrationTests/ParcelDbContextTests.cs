using AutoFixture;
using AutoFixture.Xunit2;

using Core.Dto;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using ParcelApi.Data;
using ParcelApi.Data.Abstraction;
using ParcelApi.Data.Models;

using Test.Utility;


namespace ParcelApi.IntegrationTests;

[Collection(nameof(IntegrationTestsCollection))]
public class ParcelDbContextTests : IDisposable
{
    private readonly ParcelDeliveryWebApplicationFactory<Program, IParcelDbContext, ParcelDbContext> _factory;
    private readonly ParcelDbContext _sut;

    public ParcelDbContextTests()
    {
        _factory = new();
        _factory.EnsureDbCreated();

        _sut = _factory.DbContext;
    }

    public void Dispose()
    {
        _factory.EnsureDbDeleted();
        _factory.Dispose();
    }

    [Theory, AutoData]
    public async Task TableGetters_ShouldReturnSeededData(List<Parcel> expectedParcels)
    {
        // Arrange
        await _factory.SeedDataAsync(expectedParcels);
        var expectedParcelDetails = expectedParcels.Select(x => x.ParcelDetails).ToList();
        var expectedDeliveryDetails = expectedParcels.Select(x => x.DeliveryDetails).ToList();

        // Act
        List<Parcel> actualParcels = await _sut.Parcels.ToListAsync();
        List<ParcelDetails> actualParcelDetails = await _sut.ParcelDetails.ToListAsync();
        List<DeliveryDetails> actualDeliveryDetails = await _sut.DeliveryDetails.ToListAsync();

        // Assert
        actualParcels.Should().BeEquivalentTo(expectedParcels);
        actualParcelDetails.Should().BeEquivalentTo(expectedParcelDetails);
        actualDeliveryDetails.Should().BeEquivalentTo(expectedDeliveryDetails);
    }


    [Theory, AutoData]
    public async Task GetAllParcels_ParcelsExist_ShouldReturnParcels(List<Parcel> expectedParcels)
    {
        // Arrange
        await _factory.SeedDataAsync(expectedParcels);

        // Act
        List<Parcel> actualParcels = await _sut.GetAllParcelsAsync();

        // Assert
        actualParcels.Should().BeEquivalentTo(expectedParcels);
    }

    [Fact]
    public async Task GetAllParcels_ParcelsNotExist_ShouldReturnEmptyCollection()
    {
        // Act
        List<Parcel> actualParcels = await _sut.GetAllParcelsAsync();

        // Assert
        actualParcels.Should().BeEmpty();
    }


    [Theory, AutoData]
    public async Task GetParcelById_ParcelExist_ShouldReturnParcel(List<Parcel> parcels)
    {
        // Arrange
        await _factory.SeedDataAsync(parcels);

        Parcel expectedParcel = parcels.First();

        // Act
        Parcel actualParcel = await _sut.GetParcelByIdAsync(expectedParcel.Id);

        // Assert
        actualParcel.Should().BeEquivalentTo(expectedParcel);
    }

    [Theory, AutoData]
    public async Task GetParcelById_ParcelNotExist_ShouldReturnNull(List<Parcel> parcels)
    {
        // Arrange
        await _factory.SeedDataAsync(parcels);

        // Act
        Parcel actualParcel = await _sut.GetParcelByIdAsync(0);

        // Assert
        actualParcel.Should().BeNull();
    }


    [Theory, AutoData]
    public async Task AddParcel_ShouldCreateParcelRecord(Parcel expectedParcel)
    {
        // Act
        await _sut.AddParcelAsync(expectedParcel);

        // Assert
        Parcel actualParcel = await _factory.GetData<Parcel>(expectedParcel.Id);
        actualParcel.ParcelDetails = await _factory.GetData<ParcelDetails>(expectedParcel.ParcelDetails.Id);
        actualParcel.DeliveryDetails = await _factory.GetData<DeliveryDetails>(expectedParcel.DeliveryDetails.Id);

        actualParcel.Should().BeEquivalentTo(expectedParcel);
    }


    [Theory, AutoData]
    public async Task UpdateParcel_ShouldUpdateParcelRecord(Parcel existingParcel)
    {
        // Arrange
        existingParcel.Status = ParcelStatus.Delivered;

        ParcelStatus oldStatus = existingParcel.Status;

        await _factory.SeedDataAsync([existingParcel]);

        existingParcel.Status = ParcelStatus.None;

        // Act
        await _sut.UpdateParcelAsync(existingParcel);

        // Assert
        Parcel actualParcel = await _factory.GetData<Parcel>(existingParcel.Id);

        actualParcel.Status
            .Should()
            .Be(ParcelStatus.None, "because we set it to None")
            .And.NotBe(oldStatus, "because we set it to Delivered");
    }
}