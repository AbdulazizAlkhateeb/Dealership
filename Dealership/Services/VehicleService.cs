using Dealership.DTO;
using Dealership.Entities;
using Dealership.Mapping;      
using Dealership.Repositories;

namespace Dealership.Services;

public class VehicleService : IVehicleService
{
    private readonly JsonVehicleRepository _repo;
    public VehicleService(JsonVehicleRepository repo) => _repo = repo;

    public List<VehicleDto> GetAll() =>
        _repo.GetAll().Select(v => v.ToDto()).ToList();

    public VehicleDto? GetById(int id) =>
        _repo.GetById(id)?.ToDto();

    public VehicleDto Create(VehicleDto dto)
    {
        var nextId = (_repo.GetAll().LastOrDefault()?.Id ?? 0) + 1;
        var entity = dto.ToEntity();
        entity.Id = nextId;

        _repo.Add(entity);
        return entity.ToDto();
    }

    public bool Update(int id, VehicleDto dto)
    {
        var existing = _repo.GetById(id);
        if (existing is null) return false;

        existing.Make = dto.Make;
        existing.Model = dto.Model;
        existing.Year = dto.Year;
        existing.Price = dto.Price;


        return _repo.Update(existing);
    }

    public bool Delete(int id) => _repo.Delete(id);

    public List<VehicleDto> Browse(string? make, string? model, int? minYear, int? maxYear,
                               decimal? minPrice, decimal? maxPrice, bool? inStock)
    {
        var q = _repo.GetAll().AsQueryable();

        if (!string.IsNullOrWhiteSpace(make))
            q = q.Where(v => v.Make.Contains(make, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(model))
            q = q.Where(v => v.Model.Contains(model, StringComparison.OrdinalIgnoreCase));

        if (minYear.HasValue)
            q = q.Where(v => v.Year >= minYear.Value);

        if (maxYear.HasValue)
            q = q.Where(v => v.Year <= maxYear.Value);

        if (minPrice.HasValue)
            q = q.Where(v => v.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            q = q.Where(v => v.Price <= maxPrice.Value);

        if (inStock.HasValue)
            q = q.Where(v => v.InStock == inStock.Value);

        return q.Select(v => v.ToDto()).ToList();
    }

    public VehicleDto? GetByName(string name)
    {
        return _repo.GetAll()
            .FirstOrDefault(v =>
                v.Model.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                v.Make.Contains(name, StringComparison.OrdinalIgnoreCase))
            ?.ToDto();
    }
}
