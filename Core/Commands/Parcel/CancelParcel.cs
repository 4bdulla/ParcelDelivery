using Core.Dto;


namespace Core.Commands.Parcel;

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

    /// <summary>
    /// Represents if cancel operation was performed
    /// </summary>
    public bool Canceled { get; set; }
}