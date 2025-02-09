using Core.Dto;


namespace Core.Commands.Parcel;

/// <summary>
/// Request to update the status of a parcel.
/// </summary>
public class UpdateParcelStatusRequest
{
    /// <summary>
    /// Parcel ID.
    /// </summary>
    public required int ParcelId { get; set; }

    /// <summary>
    /// New status.
    /// </summary>
    public required ParcelStatus NewStatus { get; set; }
}

/// <summary>
/// Response containing updated parcel information.
/// </summary>
public class UpdateParcelStatusResponse
{
    /// <summary>
    /// Updated parcel.
    /// </summary>
    public ParcelDto Parcel { get; set; }
}