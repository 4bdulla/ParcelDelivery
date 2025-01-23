﻿using System.ComponentModel.DataAnnotations.Schema;

using AutoMapper;

using Core.Dto;

using ParcelApi.Data.Abstraction;

namespace ParcelApi.Data.Models;

/// <summary>
/// Parcel details entity.
/// </summary>
[AutoMap(typeof(ParcelDetailsDto), ReverseMap = true)]
[Table(nameof(ParcelDbContext.ParcelDetails), Schema = nameof(ParcelApi))]
public class ParcelDetails : IUniqueItem
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public int Id { get; set; }

    // Add parcel related details here (e.g. Dimensions, Type, Delivery notes, etc.)
}