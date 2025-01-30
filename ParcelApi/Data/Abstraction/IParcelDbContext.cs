using ParcelApi.Data.Models;

namespace ParcelApi.Data.Abstraction;

/// <summary>
/// Interface for parcel database context.
/// </summary>
public interface IParcelDbContext
{
    /// <summary>
    /// Gets a parcel by its ID.
    /// </summary>
    /// <param name="id">The parcel ID.</param>
    /// <returns>The parcel.</returns>
    Task<Parcel> GetParcelByIdAsync(int id);

    /// <summary>
    /// Gets all parcels.
    /// </summary>
    /// <returns>The list of parcels.</returns>
    Task<List<Parcel>> GetAllParcelsAsync();

    /// <summary>
    /// Adds a new parcel.
    /// </summary>
    /// <param name="parcel">The parcel to add.</param>
    Task AddParcelAsync(Parcel parcel);

    /// <summary>
    /// Updates an existing parcel.
    /// </summary>
    /// <param name="parcel">The parcel to update.</param>
    Task UpdateParcelAsync(Parcel parcel);
}