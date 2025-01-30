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
    /// Assigned to courier
    /// </summary>
    Assigned = 2,

    /// <summary>
    /// Parcel being delivered
    /// </summary>
    InProgress = 3,

    /// <summary>
    /// Delivered to destination.
    /// </summary>
    Delivered = 4,

    /// <summary>
    /// Canceled.
    /// </summary>
    Canceled = 5
}