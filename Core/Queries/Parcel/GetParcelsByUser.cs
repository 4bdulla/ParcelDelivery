﻿using Core.Dto;


namespace Core.Queries.Parcel;

/// <summary>
/// Request to get parcels by user ID.
/// </summary>
public class GetParcelsByUserRequest
{
    /// <summary>
    /// User ID.
    /// </summary>
    public required int UserId { get; set; }
}

/// <summary>
/// Response containing parcels for a user.
/// </summary>
public class GetParcelsByUserResponse
{
    /// <summary>
    /// List of parcels.
    /// </summary>
    public IEnumerable<ParcelDto> Parcels { get; set; }
}