using FuelmanagementApi.Models;
using FuelmanagementApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FuelmanagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : Controller
    {
        private readonly DbCall _dbCall;

        public VehicleController(DbCall dbCall)
        {
            _dbCall = dbCall;
        }

        [Authorize]
        [HttpGet("view-all-vehicles")]
        public IActionResult Index()
        {
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                "",
                "View",
                "Vehicles"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpGet("view-vehicle/{id}")]
        public IActionResult ViewVehicle(int id) { 
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                id.ToString(),
                "View",
                "Vehicles"
            );
            return Ok(response);
        }


        [Authorize]
        [HttpPost("create-vehicle")]
        public IActionResult CreateVehicle([FromBody] DBconnect dBconnect)
        {
            var response = _dbCall.CallStoredProcedure(
                dBconnect.param1,
                dBconnect.param2,
                dBconnect.param3,
                dBconnect.param4,
                "",
                "Insert",
                "Vehicles"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpPut("update-vehicle/{id}")]
        public IActionResult UpdateVehicle(int id, [FromBody] DBconnect dBconnect) {
            var response = _dbCall.CallStoredProcedure(
               dBconnect.param1,
               dBconnect.param2,
               dBconnect.param3,
               dBconnect.param4,
               "",
               "Update",
               "Vehicles"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpGet("vehicle-refuel-history/{id}")]
        public IActionResult VehicleRefuelHistory(int id)
        {
             var response = _dbCall.CallStoredProcedure(
               "",
               "",
               "",
               "",
               id.ToString(),
               "VehicleRefuelHistory",
               "VehicleFuelHistory"
            );
            return Ok(response);
        }
    }
}
