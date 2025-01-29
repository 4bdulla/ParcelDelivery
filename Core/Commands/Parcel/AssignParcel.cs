using Core.Dto;


namespace Core.Commands.Parcel;

/// <summary>
/// Request to assign a parcel to a courier.
/// </summary>
public class AssignParcelRequest
{
    /// <summary>
    /// Parcel ID.
    /// </summary>
    public int ParcelId { get; set; }

    /// <summary>
    /// Courier ID.
    /// </summary>
    public int CourierId { get; set; }
}

/// <summary>
/// Response containing updated parcel information.
/// </summary>
public class AssignParcelResponse
{
    /// <summary>
    /// Updated parcel.
    /// </summary>
    public ParcelDto Parcel { get; set; }
}