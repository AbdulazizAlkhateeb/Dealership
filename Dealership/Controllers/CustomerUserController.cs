using Dealership.DTO;
using Dealership.Security;
using Dealership.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dealership.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerUserController : ControllerBase
    {
        private readonly IVehicleService _service;
        private readonly PurchaseService _purchas;

        public CustomerUserController(IVehicleService service, PurchaseService purchas)
        {
            _service = service;
            _purchas = purchas;
        }

        // GET api/vehicles
        [HttpGet("vehicle/getAll")]
        public ActionResult<List<VehicleDto>> GetAll() => Ok(_service.GetAll());


        [HttpGet("vehicle/browse")]
        [AllowAnonymous] 
        public ActionResult<List<VehicleDto>> Browse(
        [FromQuery] string? make,
        [FromQuery] string? model,
        [FromQuery] int? minYear,
        [FromQuery] int? maxYear,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? inStock)
        {
            var list = _service.Browse(make, model, minYear, maxYear, minPrice, maxPrice, inStock);
            return Ok(list);
    
        }

        [HttpGet("vehicle/details/byNameOrMadle/{nameOrMadle}")]
        [AllowAnonymous] 
        public ActionResult<VehicleDto> GetDetailsByName(string nameOrMadle)
        {
            var v = _service.GetByName(nameOrMadle);
            return v is null ? NotFound() : Ok(v);
        }


        [HttpPost("purchaseRequest/{vehicleId:int}")]
        [Authorize(Roles = "Customer"), OtpRequired("PurchaseRequest")]
        public ActionResult<PurchaseRequestDto> Request(int vehicleId, [FromQuery] string otp)
        {
            try
            {
                var dto = _purchas.CreateRequest(vehicleId);
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Vehicle not found.");
            }
            catch (InvalidOperationException)
            {
                return Conflict("Vehicle not available.");
            }
        }

        [HttpGet("mine")]
        [Authorize(Roles = "Customer")]
        public ActionResult<List<PurchaseRequestDto>> Mine() => Ok(_purchas.GetMine());



    }

}
