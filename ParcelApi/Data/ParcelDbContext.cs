using Microsoft.EntityFrameworkCore;

using ParcelApi.Data.Abstraction;
using ParcelApi.Data.Models;

namespace ParcelApi.Data;

/// <summary>
/// Database context for parcels.
/// </summary>
public class ParcelDbContext(DbContextOptions<ParcelDbContext> options) : DbContext(options), IParcelDbContext
{
    /// <summary>
    /// Gets or sets the parcels.
    /// </summary>
    public DbSet<Parcel> Parcels { get; set; }

    /// <summary>
    /// Gets or sets the delivery details.
    /// </summary>
    public DbSet<DeliveryDetails> DeliveryDetails { get; set; }

    /// <summary>
    /// Gets or sets the parcel details.
    /// </summary>
    public DbSet<ParcelDetails> ParcelDetails { get; set; }

    /// <summary>
    /// Gets a parcel by its ID.
    /// </summary>
    /// <param name="id">The parcel ID.</param>
    /// <returns>The parcel.</returns>
    public async Task<Parcel> GetParcelByIdAsync(int id)
    {
        Parcel parcel = await this.Parcels
            .Include(p => p.DeliveryDetails)
            .Include(p => p.ParcelDetails)
            .FirstOrDefaultAsync(p => p.Id.Equals(id));

        return parcel;
    }

    /// <summary>
    /// Gets all parcels.
    /// </summary>
    /// <returns>The list of parcels.</returns>
    public async Task<List<Parcel>> GetAllParcelsAsync()
    {
        return await this.Parcels
            .Include(p => p.DeliveryDetails)
            .Include(p => p.ParcelDetails)
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new parcel.
    /// </summary>
    /// <param name="parcel">The parcel to add.</param>
    public async Task AddParcelAsync(Parcel parcel)
    {
        await this.Parcels.AddAsync(parcel);
        await base.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing parcel.
    /// </summary>
    /// <param name="parcel">The parcel to update.</param>
    public async Task UpdateParcelAsync(Parcel parcel)
    {
        this.Parcels.Update(parcel);
        await base.SaveChangesAsync();
    }
}