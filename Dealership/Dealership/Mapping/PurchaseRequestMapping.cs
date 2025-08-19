using Dealership.DTO;
using Dealership.Entities;

namespace Dealership.Mapping;

public static class PurchaseRequestMapping
{
    public static PurchaseRequestDto ToDto(this PurchaseRequest p, Vehicle? vehicle = null) => new()
    {
        Id = p.Id,
        VehicleId = p.VehicleId,
        CustomerId = p.CustomerId,
        Status = p.Status,
        RequestedAtUtc = p.RequestedAtUtc,
        Vehicle = vehicle?.ToDto()
    };




    public static PurchaseRequest ToEntity(this PurchaseRequestDto d) => new()
    {
        Id = d.Id,
        VehicleId = d.VehicleId,
        CustomerId = d.CustomerId,
        Status = string.IsNullOrWhiteSpace(d.Status) ? "Pending" : d.Status,
        RequestedAtUtc = d.RequestedAtUtc == default ? DateTime.UtcNow : d.RequestedAtUtc
    };
}
