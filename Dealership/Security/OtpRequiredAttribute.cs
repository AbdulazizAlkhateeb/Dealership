using Dealership.DTO;
using Dealership.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dealership.Security;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class OtpRequiredAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _purpose; // "Login" | "Register" | "PurchaseRequest" | "UpdateVehicle"

    public OtpRequiredAttribute(string purpose) => _purpose = purpose;

    public async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
    {
        var req = ctx.HttpContext.Request;
        var code = req.Headers["X-OTP-Code"].FirstOrDefault()
                   ?? req.Query["otp"].FirstOrDefault();
        
        var reqBody = ctx.ActionArguments.Values.FirstOrDefault();



        if (string.IsNullOrWhiteSpace(code))
        {
            ctx.Result = new UnauthorizedObjectResult("OTP required");
            return;
        }

        string? userName = null;

        if (ctx.HttpContext.User?.Identity?.IsAuthenticated == true)
        {
            userName = ctx.HttpContext.User.Identity!.Name;
        }
        else
        {
            userName = req.Headers["X-UserName"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(userName))
            {
                var dtoArg = ctx.ActionArguments.Values.FirstOrDefault(v => v is UserDto) as UserDto;
                userName = dtoArg?.UserName;
            }
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            ctx.Result = new BadRequestObjectResult("UserName required for OTP");
            return;
        }

        var otp = ctx.HttpContext.RequestServices.GetRequiredService<OtpService>();
        if (!otp.Validate(userName!, _purpose, code!, true))
        {
            ctx.Result = new UnauthorizedObjectResult("Invalid or expired OTP");
            return;
        }

        await next();
    }

}