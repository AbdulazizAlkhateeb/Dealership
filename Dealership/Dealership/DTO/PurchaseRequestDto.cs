
namespace Dealership.DTO;

public class PurchaseRequestDto
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public int CustomerId { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime RequestedAtUtc { get; set; }
    public VehicleDto Vehicle { get; set; }
}
