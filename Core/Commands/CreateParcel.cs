﻿namespace Core.Commands;

using Core.Dto;

/// <summary>
/// Request to create a new parcel.
/// </summary>
public class CreateParcelRequest
{
    /// <summary>
    /// User ID.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Source address.
    /// </summary>
    public string SourceAddress { get; set; }

    /// <summary>
    /// Destination address.
    /// </summary>
    public string DestinationAddress { get; set; }
}

/// <summary>
/// Response containing created parcel information.
/// </summary>
public class CreateParcelResponse
{
    /// <summary>
    /// Created parcel.
    /// </summary>
    public ParcelDto Parcel { get; set; }
}