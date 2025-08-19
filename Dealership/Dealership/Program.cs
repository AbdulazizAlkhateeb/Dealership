
using Dealership.Repositories;
using Dealership.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Dealership
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<JsonVehicleRepository>();
            builder.Services.AddScoped<JsonPurchaseRequestRepository>();
            builder.Services.AddScoped<JsonUserRepository>();
            builder.Services.AddScoped<JsonOtpRepository>();

            builder.Services.AddScoped<IVehicleService, VehicleService>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<PurchaseService>();
            builder.Services.AddScoped<OtpService>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(o =>
            {
                o.Cookie.Name = "dealership.auth";
                o.SlidingExpiration = true;
                o.LoginPath = "/auth/login";        // not used by Swagger redirects, OK
                o.AccessDeniedPath = "/auth/denied";
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
            });

            var app = builder.Build();

            app.MapOpenApi();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/openapi/v1.json", "Dealership v1");
                c.RoutePrefix = "swagger"; // UI at /swagger
            });

            app.Run();
        }
    }
}
