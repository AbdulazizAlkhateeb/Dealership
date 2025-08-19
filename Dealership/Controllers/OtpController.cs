using Dealership.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dealership.Controllers;
[ApiController]
[Route("api/otp")]
public class OtpController : ControllerBase
{
    private readonly OtpService _otp;
    public OtpController(OtpService otp) => _otp = otp;

    public record OtpRequest(string UserName, string Purpose);

    [HttpPost("request")]
    public IActionResult RequestOtp([FromBody] OtpRequest r)
    {
        var (code, exp) = _otp.Generate(r.UserName, r.Purpose);
        return Ok(new { message = "OTP generated", code, expiresAt = exp });
    }
}
