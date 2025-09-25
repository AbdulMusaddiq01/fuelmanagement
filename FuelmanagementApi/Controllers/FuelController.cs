using FuelmanagementApi.Models;
using FuelmanagementApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FuelmanagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuelController : Controller
    {
        private readonly PrepareRequest _prepareRequest;
        private readonly DbCall _dbCall;
        public FuelController(PrepareRequest prepareRequest, DbCall dbCall)
        {
            _prepareRequest = prepareRequest;
            _dbCall = dbCall;
        }

        //[Authorize]
        [HttpPost("fuel-types")]
        public IActionResult Index([FromBody] DBconnect dBconnect)
        {
            var request = _prepareRequest.PrepareReq(dBconnect);
            return Ok(request);
        }

        //[Authorize]
        [HttpGet("view-fuel-type/{id}")]
        public IActionResult View(int id)
        {
            var dbconnect = new DBconnect
            {
                param1 = "",
                param2 = "",
                param3 = "",
                param4 = "",
                search = id.ToString(),
                operation = "View",
                screen = "FuelTypes",
            };
            var request = _prepareRequest.PrepareReq(dbconnect);

            return Ok(request);
        }


        // Fuel Tank Issues CRUD Starts Here..
        [Authorize]
        [HttpGet("view-tanks-history")]
        public IActionResult GetAllTanksIssueHistory()
        {
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                "",
                "",
                "TankIssued"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpGet("view-tank-history/{id}")]
        public IActionResult ViewTankHistory(int id)
        {
            ;
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                id.ToString(),
                "View",
                "TankIssued"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpGet("view-vehicle-refill-history/{id}")]
        public IActionResult ViewVehicleRefillHistory(int id)
        {
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                id.ToString(),
                "VehicleRefillHistory",
                "TankIssued"
            );

            return Ok(response);
        }

        [Authorize]
        [HttpPost("issue-fuel")]
        public IActionResult IssueFuel([FromBody] DBconnect dBconnect)
        {
            // Please check the details carefully. Once fuel is issued, this record cannot be edited. 
            var response = _dbCall.CallStoredProcedure(
                dBconnect.param1,
                dBconnect.param2,
                dBconnect.param3,
                dBconnect.param4,
                dBconnect.search,
                "Insert",
                "TankIssued"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpPut("update-issue-fuel/{id}")]
        public IActionResult UpdateIssueFuel([FromBody] DBconnect dBconnect)
        {
            var response = _dbCall.CallStoredProcedure(
                dBconnect.param1,
                dBconnect.param2,
                dBconnect.param3,
                dBconnect.param4,
                dBconnect.search,
                "Update",
                "TankIssued"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpGet("view-issue-fuel-detail/{id}")]
        public IActionResult ViewIssueFuelDetail(int id)
        {
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                id.ToString(),
                "FuelIssueDetail",
                "TankIssued"
            );
            return Ok(response);
        }



        [Authorize]
        [HttpGet("fuel-issue-history")]
        public IActionResult FuelIssueHistory()
        {
            // Please check the details carefully. Once fuel is issued, this record cannot be edited. 
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                "",
                "View",
                "TankIssued"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpGet("check-vehicle-last-fueled/{id}")]
        public IActionResult checkVehicleLastFueled(int id)
        {
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                id.ToString(),
                "CheckVehicleLastFueled",
                "TankIssued"
            );
            return Ok(response);
        }
    }
}
