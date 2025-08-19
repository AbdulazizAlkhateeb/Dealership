// Controllers/AuthController.cs
using Dealership.DTO;
using Dealership.Security;
using Dealership.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace Dealership.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;
    private readonly OtpService _otp;

    public AuthController(AuthService auth, OtpService otp)
    {
        _auth = auth;
        _otp = otp;
    }

    [HttpPost("register")]
    [AllowAnonymous, OtpRequired("Register")]
    public ActionResult<UserDto> Register([FromBody] UserDto dto, [FromQuery] string otp)
    {
        //var userDto = new UserDto
        //{
        //    UserName = dto.UserName,
        //    Password = dto.Password,
        //    Role = dto.Role
        //};
        return Ok(_auth.Register(dto));
    }
    //} => Ok(_auth.Register(dto));


    [HttpPost("login")]
    [AllowAnonymous, OtpRequired("Login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] UserDto dto, [FromQuery] string otp)
    {

        //var c = new UserDto
        //{
        //    UserName = dto.UserName,
        //    Password = dto.Password
        //};
        var u = await _auth.LoginAsync(dto);
        return u is null ? Unauthorized("Invalid credentials") : Ok(u);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    { await _auth.LogoutAsync(); return NoContent(); }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<UserDto> Me()
        => _auth.Me() is { } me ? Ok(me) : Unauthorized();
}
