namespace Dealership.Entities;

public class PurchaseRequest
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public int CustomerId { get; set; }
    public string Status { get; set; } = "Pending";   // Pending | Approved | Rejected
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
}
