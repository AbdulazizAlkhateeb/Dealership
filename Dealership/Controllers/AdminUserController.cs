using Dealership.DTO;
using Dealership.Security;
using Dealership.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace Dealership.Controllers
{

    [Route("api/")]
    [ApiController]
    [Authorize] 
    public class AdminUserController : ControllerBase
    {

        private readonly IVehicleService _service;
        private readonly AuthService _auth;
        private readonly PurchaseService _purchas;


        public AdminUserController(IVehicleService service, AuthService auth, PurchaseService purchas)
        {
            _service = service;
            _auth = auth;
            _purchas = purchas;
        }




        [HttpGet("customers")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<UserDto>> GetCustomers() => Ok(_auth.GetUsersByRole("Customer"));



        [HttpGet("vehicle/getAll")]
        public ActionResult<List<VehicleDto>> GetAll() => Ok(_service.GetAll());


        [HttpGet("vehicle/getBy/{id}")]
        public IActionResult GetById(int id)
        {
            var vehicle = _service.GetById(id);
            if (vehicle == null) return NotFound();
            return Ok(vehicle);
        }


        [HttpPost("vehicle/create")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] VehicleDto dto)
        {
            var created = _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }


        [HttpPut("vehicle/update")]
        [Authorize(Roles = "admin"), OtpRequired("UpdateVehicle")]
        //[OtpRequired("UpdateVehicle")]
        public IActionResult Update([FromBody] VehicleDto dto, [FromQuery] string otp)
        {
            if (dto == null || dto.Id <= 0)
                return BadRequest("Invalid vehicle data.");

            var ok = _service.Update(dto.Id, dto);

            if (!ok) return NotFound();


            var updated = _service.GetById(dto.Id);
            return StatusCode(200, updated); 
        }


        [HttpDelete("vehicle/deleteBy/{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var ok = _service.Delete(id);
            return ok ? NoContent() : NotFound();
        }


        [HttpGet("allPurchases")]
        [Authorize(Roles = "Admin")]
        public ActionResult<List<PurchaseRequestDto>> All()
        => Ok(_purchas.GetAll());


        [HttpGet("purchaseById/{id:int}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<PurchaseRequestDto> ById(int id)
            => _purchas.GetById(id) is { } pr ? Ok(pr) : NotFound();

        [HttpPost("purchase/{id:int}/approve")]
        [Authorize(Roles = "Admin")]
        public IActionResult Approve(int id)
            => _purchas.Approve(id) ? Ok() : Conflict("Cannot approve.");


        [HttpPost("purchase/{id:int}/reject")]
        [Authorize(Roles = "Admin")]
        public IActionResult Reject(int id)
            => _purchas.Reject(id) ? Ok() : Conflict("Cannot reject.");
    }
}

