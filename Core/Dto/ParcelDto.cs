namespace Core.Dto;

/// <summary>
/// Parcel details.
/// </summary>
public class ParcelDto
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Status.
    /// </summary>
    public ParcelStatus Status { get; set; }

    /// <summary>
    /// User ID.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Courier ID.
    /// </summary>
    public int? CourierId { get; set; }

    /// <summary>
    /// Parcel details.
    /// </summary>
    public ParcelDetailsDto ParcelDetails { get; set; }

    /// <summary>
    /// Delivery details.
    /// </summary>
    public DeliveryDetailsDto DeliveryDetails { get; set; }
}