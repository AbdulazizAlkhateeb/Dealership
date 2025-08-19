using Dealership.DTO;
using Dealership.Entities;

namespace Dealership.Mapping;

public static class UserMapping
{
    public static UserDto ToDto(this User u) => new()
    {

        UserName = u.UserName,
        Role = u.Role
    };

    public static User ToEntity(this UserDto d) => new()
    {
        Id = d.Id,
        UserName = d.UserName,
        Role = string.IsNullOrWhiteSpace(d.Role) ? "Customer" : d.Role,
        PasswordHash = string.IsNullOrWhiteSpace(d.Password)
            ? ""
            : BCrypt.Net.BCrypt.HashPassword(d.Password)
    };

}
