using System.Security.Cryptography;
using System.Text;
using Dealership.Entities;
using Dealership.Repositories;
using Microsoft.Extensions.Logging;

namespace Dealership.Services;
public class OtpService
{
    private readonly JsonOtpRepository _repo;
    private readonly ILogger<OtpService> _log;
    public OtpService(JsonOtpRepository repo, ILogger<OtpService> log)
    { _repo = repo; _log = log; }

    public (string code, DateTime expiresAt) Generate(string userName, string purpose, int ttlMinutes = 3)
    {
        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6"); // 6 digits
        var salt = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
        var hash = Hash(code, salt);

        var token = new OtpToken
        {
            UserName = userName,
            Purpose = purpose,
            Salt = salt,
            CodeHash = hash,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(ttlMinutes)
        };
        _repo.Add(token);

        _log.LogInformation("OTP [{Purpose}] for {User}: {Code} (exp {Exp:u})",
            purpose, userName, code, token.ExpiresAtUtc);

        return (code, token.ExpiresAtUtc);
    }

    public bool Validate(string userName, string purpose, string code, bool consume)
    {
        var t = _repo.GetLatestValid(userName, purpose);
        if (t is null) return false;

        var ok = SlowEquals(t.CodeHash, Hash(code, t.Salt));
        if (!ok) return false;

        _repo.MarkUsed(t);
        return true;
    }

    private static string Hash(string code, string salt)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes($"{salt}:{code}"));
        return Convert.ToHexString(bytes);
    }

    private static bool SlowEquals(string a, string b)
    {
        var ba = Convert.FromHexString(a);
        var bb = Convert.FromHexString(b);
        var diff = ba.Length ^ bb.Length;
        for (int i = 0; i < Math.Min(ba.Length, bb.Length); i++)
            diff |= ba[i] ^ bb[i];
        return diff == 0;
    }
}

