namespace Core.Common;

public class ParcelNotFoundResponse
{
    public int? ParcelId { get; set; }
    public string Message => $"Parcel not found by ID: {this.ParcelId}";
}