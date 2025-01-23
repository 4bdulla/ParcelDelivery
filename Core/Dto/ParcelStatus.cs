namespace Core.Dto;

/// <summary>
/// Status of a parcel.
/// </summary>
public enum ParcelStatus
{
    /// <summary>
    /// Not specified.
    /// </summary>
    None = 0,

    /// <summary>
    /// Pending delivery.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Delivered.
    /// </summary>
    Delivered = 2,

    /// <summary>
    /// Canceled.
    /// </summary>
    Canceled = 3
}