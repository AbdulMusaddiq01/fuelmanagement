using FuelmanagementApi.Models;
using FuelmanagementApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FuelmanagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : Controller
    {
        public readonly DbCall _dbCall;

        public ReportsController(DbCall dbCall)
        {
            _dbCall = dbCall;
        }

        [Authorize]
        [HttpPost("tank-history")]
        public async Task<IActionResult> TankHistory([FromBody] DBconnect dBconnect)
        {
            var response = _dbCall.CallStoredProcedure(
                dBconnect.param1,
                dBconnect.param2,
                dBconnect.param3,
                dBconnect.param4,
                dBconnect.search,
                "TankHistory",
                "Reports"
            );
            return Ok(response);
        }


        [Authorize]
        [HttpPost("vehicle-refill-history")]
        public async Task<IActionResult> VehicleRefillHistory([FromBody] DBconnect dBconnect)
        {
            var response = _dbCall.CallStoredProcedure(
                dBconnect.param1,
                dBconnect.param2,
                dBconnect.param3,
                dBconnect.param4,
                dBconnect.search,
                "VehicleRefillHistory",
                "Reports"
            );
            return Ok(response);
        }


        [Authorize]
        [HttpPost("isssue-history")]
        public async Task<IActionResult> IssueHistory([FromBody] DBconnect dBconnect)
        {
            var response = _dbCall.CallStoredProcedure(
                dBconnect.param1,
                dBconnect.param2,
                dBconnect.param3,
                dBconnect.param4,
                dBconnect.search,
                "IssueHistory",
                "Reports"
            );
            return Ok(response);
        }
    }
}
