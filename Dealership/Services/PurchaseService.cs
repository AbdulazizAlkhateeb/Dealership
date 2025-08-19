using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Dealership.DTO;
using Dealership.Entities;
using Dealership.Mapping;
using Dealership.Repositories;

namespace Dealership.Services;

public class PurchaseService
{
    private readonly JsonPurchaseRequestRepository _repo;
    private readonly JsonVehicleRepository _vehicles;
    private readonly IHttpContextAccessor _http;

    public PurchaseService(
        JsonPurchaseRequestRepository repo,
        JsonVehicleRepository vehicles,
        IHttpContextAccessor http)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _vehicles = vehicles ?? throw new ArgumentNullException(nameof(vehicles));
        _http = http ?? throw new ArgumentNullException(nameof(http));
    }

    private int CurrentUserId()
    {
        var id = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(id, out var userId))
            throw new UnauthorizedAccessException("Not authenticated.");
        return userId;
    }


    public PurchaseRequestDto CreateRequest(int vehicleId)
    {
        var v = _vehicles.GetById(vehicleId);
        if (v is null)
            throw new KeyNotFoundException("Vehicle not found.");

        if (!v.InStock)
            throw new InvalidOperationException("Vehicle not available.");

        var entity = new PurchaseRequest
        {
            VehicleId = vehicleId,
            CustomerId = CurrentUserId(),
            Status = "Pending",
            RequestedAtUtc = DateTime.UtcNow
        };

        entity = _repo.Add(entity);
        return entity.ToDto(v);
    }


    public bool Approve(int requestId)
    {
        var pr = _repo.GetById(requestId);
        if (pr is null || pr.Status != "Pending") return false;

        var v = _vehicles.GetById(pr.VehicleId);
        if (v is null) return false;
        if (!v.InStock) return false;

        pr.Status = "Approved";
        var okReq = _repo.Update(pr);

        v.InStock = false;
        var okVeh = _vehicles.Update(v);

        return okReq && okVeh;
    }


    public bool Reject(int requestId)
    {
        var pr = _repo.GetById(requestId);
        if (pr is null || pr.Status != "Pending") return false;

        pr.Status = "Rejected";
        return _repo.Update(pr);
    }

    public IEnumerable<PurchaseRequestDto> GetAll()
    {
        var entities = _repo.GetAll();

        return entities.Select(p =>
        {
            var vehicle = _vehicles.GetById(p.VehicleId);
            return p.ToDto(vehicle);
        });
    }
    public List<PurchaseRequestDto> GetMine()
    {
        var me = CurrentUserId();
        return _repo.GetAll()
            .Where(x => x.CustomerId == me)
            .Select(p =>
            {
                var vehicle = _vehicles.GetById(p.VehicleId);
                return p.ToDto(vehicle);
            })
            .ToList();
    }


    public PurchaseRequestDto? GetById(int id) =>
        _repo.GetById(id)?.ToDto();
}
