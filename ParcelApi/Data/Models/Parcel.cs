using System.ComponentModel.DataAnnotations.Schema;

using AutoMapper;

using Core.Dto;

using ParcelApi.Data.Abstraction;


namespace ParcelApi.Data.Models;

/// <summary>
/// Parcel entity.
/// </summary>
[AutoMap(typeof(ParcelDto), ReverseMap = true)]
[Table(nameof(ParcelDbContext.Parcels), Schema = nameof(ParcelApi))]
public class Parcel : IUniqueItem, ICloneable
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Status.
    /// </summary>
    public ParcelStatus Status { get; set; }

    /// <summary>
    /// User ID.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Courier ID.
    /// </summary>
    public int? CourierId { get; set; }

    /// <summary>
    /// Parcel details.
    /// </summary>
    public ParcelDetails ParcelDetails { get; set; }

    /// <summary>
    /// Delivery details.
    /// </summary>
    public DeliveryDetails DeliveryDetails { get; set; }

    public object Clone() => base.MemberwiseClone();
}