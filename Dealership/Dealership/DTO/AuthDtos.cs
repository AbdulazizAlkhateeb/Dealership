namespace Dealership.DTO;

public class UserDto
{
    public int Id { get; set; }       
    public string UserName { get; set; } = ""; 
    public string? Password { get; set; }      
    public string Role { get; set; } = "Customer";
}

public class RegisterDto
{
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = "Customer";
}

public class LoginDto
{
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
}

