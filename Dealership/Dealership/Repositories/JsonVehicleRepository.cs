using Dealership.Entities;
using System.Text.Json;

namespace Dealership.Repositories;
public class JsonVehicleRepository
{
    private readonly string _path;
    private readonly JsonSerializerOptions _opts = new() { WriteIndented = true };
    private readonly List<Vehicle> _cache;

    public JsonVehicleRepository(IWebHostEnvironment env)
    {
        _path = Path.Combine(env.ContentRootPath, "App_Data", "vehicles.json");

        if (!File.Exists(_path))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.WriteAllText(_path, "[]");
        }

        var json = File.ReadAllText(_path);
        _cache = JsonSerializer.Deserialize<List<Vehicle>>(json, _opts) ?? new();
    }

    public List<Vehicle> GetAll() => _cache;

    public Vehicle? GetById(int id) => _cache.FirstOrDefault(v => v.Id == id);

    public Vehicle Add(Vehicle vehicle)
    {
        _cache.Add(vehicle);
        Persist();
        return vehicle;
    }

    public bool Update(Vehicle vehicle)
    {
        var idx = _cache.FindIndex(v => v.Id == vehicle.Id);
        if (idx < 0) return false;
        _cache[idx] = vehicle;
        Persist();
        return true;
    }

    public bool Delete(int id)
    {
        var removed = _cache.RemoveAll(v => v.Id == id) > 0;
        if (removed) Persist();
        return removed;
    }

    private void Persist() =>
        File.WriteAllText(_path, JsonSerializer.Serialize(_cache, _opts));
}
