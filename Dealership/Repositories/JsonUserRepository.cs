using System.Text.Json;
using Dealership.Entities;

namespace Dealership.Repositories;
public class JsonUserRepository
{
    private readonly string _path;
    private readonly JsonSerializerOptions _opts = new() { WriteIndented = true };
    private readonly List<User> _cache;

    public JsonUserRepository(IWebHostEnvironment env)
    {
        _path = Path.Combine(env.ContentRootPath, "App_Data", "users.json");
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        if (!File.Exists(_path)) File.WriteAllText(_path, "[]");
        _cache = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(_path)) ?? new();
    }

    public List<User> GetAll() => _cache;
    public User? GetByUserName(string userName) =>
        _cache.FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

    public User Add(User u)
    {
        u.Id = (_cache.LastOrDefault()?.Id ?? 0) + 1;
        _cache.Add(u);
        Persist();
        return u;
    }

    private void Persist() => File.WriteAllText(_path, JsonSerializer.Serialize(_cache, _opts));
}
