using Dealership.DTO;
using Dealership.Entities;

namespace Dealership.Mapping;

public static class VehicleMapping
{
    public static VehicleDto ToDto(this Vehicle v) => new()
    {
        Id = v.Id,
        Make = v.Make,
        Model = v.Model,
        Year = v.Year,
        Price = v.Price,
        InStock = v.InStock
    };

    public static Vehicle ToEntity(this VehicleCreateDto d) => new()
    {
        Make = d.Make,
        Model = d.Model,
        Year = d.Year,
        Price = d.Price,
        InStock = d.InStock
    };

    public static void Apply(this Vehicle entity, VehicleDto d)
    {
        entity.Make = d.Make;
        entity.Model = d.Model;
        entity.Year = d.Year;
        entity.Price = d.Price;
        entity.InStock = d.InStock;
    }
}
