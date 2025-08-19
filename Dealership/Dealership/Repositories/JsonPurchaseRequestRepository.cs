using System.Text.Json;
using Dealership.Entities;

namespace Dealership.Repositories;

public class JsonPurchaseRequestRepository
{
    private readonly string _path;
    private readonly JsonSerializerOptions _opts = new() { WriteIndented = true };
    private readonly List<PurchaseRequest> _cache;

    public JsonPurchaseRequestRepository(IWebHostEnvironment env)
    {
        _path = Path.Combine(env.ContentRootPath, "App_Data", "purchaseRequests.json");
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        if (!File.Exists(_path)) File.WriteAllText(_path, "[]");
        _cache = JsonSerializer.Deserialize<List<PurchaseRequest>>(File.ReadAllText(_path)) ?? new();
    }

    public List<PurchaseRequest> GetAll() => _cache;

    public PurchaseRequest? GetById(int id) => _cache.FirstOrDefault(x => x.Id == id);

    public PurchaseRequest Add(PurchaseRequest p)
    {
        p.Id = (_cache.LastOrDefault()?.Id ?? 0) + 1;
        _cache.Add(p);
        Persist();
        return p;
    }

    public bool Update(PurchaseRequest p)
    {
        var i = _cache.FindIndex(x => x.Id == p.Id);
        if (i < 0) return false;
        _cache[i] = p;
        Persist();
        return true;
    }

    private void Persist() =>
        File.WriteAllText(_path, JsonSerializer.Serialize(_cache, _opts));
}
