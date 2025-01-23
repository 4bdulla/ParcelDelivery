namespace Core.Commands;

using Core.Dto;

/// <summary>
/// Request to cancel a parcel.
/// </summary>
public class CancelParcelRequest
{
    /// <summary>
    /// Parcel ID.
    /// </summary>
    public int ParcelId { get; set; }
}

/// <summary>
/// Response containing canceled parcel information.
/// </summary>
public class CancelParcelResponse
{
    /// <summary>
    /// Canceled parcel.
    /// </summary>
    public ParcelDto Parcel { get; set; }
}