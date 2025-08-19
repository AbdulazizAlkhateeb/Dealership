using Dealership.DTO;
namespace Dealership.Services;
public interface IVehicleService
{
    List<VehicleDto> GetAll();
    VehicleDto? GetById(int id);
    VehicleDto Create(VehicleDto dto);
    bool Update(int id, VehicleDto dto);
    bool Delete(int id);

    List<VehicleDto> Browse(string? make, string? model, int? minYear, int? maxYear, decimal? minPrice, decimal? maxPrice, bool? inStock);
    VehicleDto? GetByName(string name);
}

