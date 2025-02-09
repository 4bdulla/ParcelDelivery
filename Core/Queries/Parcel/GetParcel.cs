using Core.Dto;


namespace Core.Queries.Parcel;

/// <summary>
/// Request to get a parcel by ID.
/// </summary>
public class GetParcelRequest
{
    /// <summary>
    /// Parcel ID.
    /// </summary>
    public required int ParcelId { get; set; }
}

/// <summary>
/// Response containing parcel information.
/// </summary>
public class GetParcelResponse
{
    /// <summary>
    /// Parcel details.
    /// </summary>
    public ParcelDto Parcel { get; set; }
}