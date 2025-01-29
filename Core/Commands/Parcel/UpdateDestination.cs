using Core.Dto;


namespace Core.Commands.Parcel;

/// <summary>
/// Request to update the destination of a parcel.
/// </summary>
public class UpdateDestinationRequest
{
    /// <summary>
    /// Parcel ID.
    /// </summary>
    public int ParcelId { get; set; }

    /// <summary>
    /// New destination.
    /// </summary>
    public string NewDestination { get; set; }
}

/// <summary>
/// Response containing updated parcel information.
/// </summary>
public class UpdateDestinationResponse
{
    /// <summary>
    /// Updated parcel.
    /// </summary>
    public ParcelDto Parcel { get; set; }
}