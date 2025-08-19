namespace Dealership.Entities;
public class OtpToken
{
    public int Id { get; set; }
    public string UserName { get; set; } = "";
    public string Purpose { get; set; } = ""; // Register | Login | PurchaseRequest | UpdateVehicle
    public string CodeHash { get; set; } = ""; 
    public string Salt { get; set; } = "";    
    public DateTime ExpiresAtUtc { get; set; }
    public bool Used { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UsedAtUtc { get; set; }
}
