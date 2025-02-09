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
    public required int ParcelId { get; set; }

    /// <summary>
    /// New destination.
    /// </summary>
    public required string NewDestination { get; set; }
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

    /// <summary>
    /// Represents if update destination operation was performed
    /// </summary>
    public bool Updated { get; set; }
}