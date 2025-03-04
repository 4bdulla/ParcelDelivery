﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using AutoMapper;

using Core.Dto;


namespace ParcelApi.Data.Models;

/// <summary>
/// Delivery details entity.
/// </summary>
[AutoMap(typeof(DeliveryDetailsDto), ReverseMap = true)]
[Table(nameof(ParcelDbContext.DeliveryDetails), Schema = nameof(ParcelApi))]
public class DeliveryDetails
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Source address.
    /// </summary>
    [MaxLength(100)]
    public string SourceAddress { get; set; }

    /// <summary>
    /// Destination address.
    /// </summary>
    [MaxLength(100)]
    public string DestinationAddress { get; set; }
}