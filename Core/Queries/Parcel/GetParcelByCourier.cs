using Core.Dto;


namespace Core.Queries.Parcel;

/// <summary>
/// Request to get parcels by courier ID.
/// </summary>
public class GetParcelsByCourierRequest
{
    /// <summary>
    /// Courier ID.
    /// </summary>
    public int CourierId { get; set; }
}

/// <summary>
/// Response containing parcels for a courier.
/// </summary>
public class GetParcelsByCourierResponse
{
    /// <summary>
    /// List of parcels.
    /// </summary>
    public IEnumerable<ParcelDto> Parcels { get; set; }
}