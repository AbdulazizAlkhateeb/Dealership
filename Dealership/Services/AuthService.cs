using System.Security.Claims;
using Dealership.DTO;
using Dealership.Entities;
using Dealership.Mapping;
using Dealership.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Dealership.Services;

public class AuthService 
{
    private readonly JsonUserRepository _users;
    private readonly IHttpContextAccessor _http;

    public AuthService(JsonUserRepository users, IHttpContextAccessor http)
    {
        _users = users;
        _http = http;
    }


    public UserDto Register(UserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Password))
            throw new ArgumentException("UserName and Password are required.");

        if (_users.GetByUserName(dto.UserName) is not null)
            throw new InvalidOperationException("Username already exists.");

        var entity = dto.ToEntity();       
        entity = _users.Add(entity);

        return entity.ToDto();
    }


    public async Task<UserDto?> LoginAsync(UserDto dto)
    {
        var user = _users.GetByUserName(dto.UserName);
        if (user is null) return null;

        if (!BCrypt.Net.BCrypt.Verify(dto.Password ?? "", user.PasswordHash))
            return null;

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await _http.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true });

        return user.ToDto();
    }

    public async Task LogoutAsync() =>
        await _http.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    public UserDto? Me()
    {
        var u = _http.HttpContext?.User;
        if (u?.Identity is null || !u.Identity.IsAuthenticated) return null;

        return new UserDto
        {
            Id = int.Parse(u.FindFirstValue(ClaimTypes.NameIdentifier)!),
            UserName = u.Identity.Name ?? "",
            Role = u.FindFirstValue(ClaimTypes.Role) ?? "Customer"

        };
    }

    public List<UserDto> GetUsersByRole(string role)
    {
        return _users
            .GetAll()
            .Where(u => string.Equals(u.Role, role, StringComparison.OrdinalIgnoreCase))
            .Select(u => u.ToDto())
            .ToList();
    }



}
