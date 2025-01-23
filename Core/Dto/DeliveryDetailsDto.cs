namespace Core.Dto;

/// <summary>
/// Delivery details of a parcel.
/// </summary>
public class DeliveryDetailsDto
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Source address.
    /// </summary>
    public string SourceAddress { get; set; }

    /// <summary>
    /// Destination address.
    /// </summary>
    public string DestinationAddress { get; set; }
}