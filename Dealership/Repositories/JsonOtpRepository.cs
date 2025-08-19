using System.Text.Json;
using Dealership.Entities;

namespace Dealership.Repositories;
public class JsonOtpRepository
{
    private readonly string _path;
    private readonly JsonSerializerOptions _opts = new() { WriteIndented = true };
    private readonly List<OtpToken> _cache;

    public JsonOtpRepository(IWebHostEnvironment env)
    {
        _path = Path.Combine(env.ContentRootPath, "App_Data", "otps.json");
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        if (!File.Exists(_path)) File.WriteAllText(_path, "[]");
        _cache = JsonSerializer.Deserialize<List<OtpToken>>(File.ReadAllText(_path)) ?? new();
    }

    public OtpToken Add(OtpToken t)
    {
        t.Id = (_cache.LastOrDefault()?.Id ?? 0) + 1;
        _cache.Add(t);
        Persist();
        return t;
    }

    public OtpToken? GetLatestValid(string userName, string purpose)
    {
        var validOtps = new List<OtpToken>();

        foreach (var x in _cache)
        {
            if (!x.Used)
            {
                if (x.UserName.ToLower().Equals(userName.ToLower()) && x.Purpose.ToLower().Equals(purpose.ToLower()) && x.ExpiresAtUtc > DateTime.UtcNow)
                {
                    validOtps.Add(x);
                }
            }

        }

        var result = validOtps
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();

        return result;
    }

    public void MarkUsed(OtpToken t)
    {
        t.Used = true;
        t.UsedAtUtc = DateTime.UtcNow;
        Persist();
    }

    private void Persist() =>
        File.WriteAllText(_path, JsonSerializer.Serialize(_cache, _opts));
}

