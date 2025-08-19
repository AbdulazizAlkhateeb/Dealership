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

    public static Vehicle ToEntity(this VehicleDto d) => new()
    {
        Id = d.Id,
        Make = d.Make,
        Model = d.Model,
        Year = d.Year,
        Price = d.Price,
        InStock = true 
    };
}
