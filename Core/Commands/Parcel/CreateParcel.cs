using Core.Dto;


namespace Core.Commands.Parcel;

/// <summary>
/// Request to create a new parcel.
/// </summary>
public class CreateParcelRequest
{
    /// <summary>
    /// User ID.
    /// </summary>
    public required int UserId { get; set; }

    /// <summary>
    /// Source address.
    /// </summary>
    public required string SourceAddress { get; set; }

    /// <summary>
    /// Destination address.
    /// </summary>
    public required string DestinationAddress { get; set; }
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