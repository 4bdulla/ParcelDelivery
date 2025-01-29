using Core.Dto;


namespace Core.Queries.Parcel;

/// <summary>
/// Request to get all parcels.
/// </summary>
public class GetParcelsRequest;

/// <summary>
/// Response containing all parcels.
/// </summary>
public class GetParcelsResponse
{
    /// <summary>
    /// List of parcels.
    /// </summary>
    public IEnumerable<ParcelDto> Parcels { get; set; }
}