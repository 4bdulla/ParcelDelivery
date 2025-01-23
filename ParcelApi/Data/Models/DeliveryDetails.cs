using System.ComponentModel.DataAnnotations.Schema;

using AutoMapper;

using Core.Dto;

using ParcelApi.Data.Abstraction;

namespace ParcelApi.Data.Models;

/// <summary>
/// Delivery details entity.
/// </summary>
[AutoMap(typeof(DeliveryDetailsDto), ReverseMap = true)]
[Table(nameof(ParcelDbContext.DeliveryDetails), Schema = nameof(ParcelApi))]
public class DeliveryDetails : IUniqueItem
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Source address.
    /// </summary>
    public string SourceAddress { get; set; }

    /// <summary>
    /// Destination address.
    /// </summary>
    public string DestinationAddress { get; set; }
}