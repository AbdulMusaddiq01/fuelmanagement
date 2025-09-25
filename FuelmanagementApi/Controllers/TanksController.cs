using System.Text.Json;
using FuelmanagementApi.Models;
using FuelmanagementApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FuelmanagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TanksController : Controller
    {

        private readonly PrepareRequest _prepareRequest;
        private readonly DbCall _dbCall;
        public TanksController(DbCall dbCall , PrepareRequest prepareRequest)
        {
            _dbCall = dbCall;
            _prepareRequest = prepareRequest;
        }


        private class TankDto
        {
            public int id {get;set;}
            public string code { get;set;}
            public string fuel_type {get;set;}
            public int tank_size { get;set;}
            public string uom { get;set;}
            public int available_qty { get;set;}
        }

        // Tanks CRUD Starts Here..
        [Authorize]
        [HttpGet("viewall")]
        public IActionResult GetAllTanks()
        {
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                "",
                "View",
                "Tanks"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpGet("view/{id}")]
        public IActionResult GetTankById(int id) 
        {
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                id.ToString(),
                "View",
                "Tanks"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpPost("create")]
        public IActionResult CreateTank([FromBody] DBconnect dBconnect) 
        {
            var response = _dbCall.CallStoredProcedure(
                dBconnect.param1,
                dBconnect.param2,
                dBconnect.param3,
                dBconnect.param4,
                "",
                "Insert",
                "Tanks"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpPut("update/{id}")]
        public IActionResult UpdateTank(int id, [FromBody] DBconnect dBconnect)
        {
            var tankdata = JsonSerializer.Deserialize<TankDto>(dBconnect.param1);
            if(tankdata.id != id)
            return BadRequest("Route ID and body ID mismatch.");

            var response = _dbCall.CallStoredProcedure(
                dBconnect.param1,
                dBconnect.param2,
                dBconnect.param3,
                dBconnect.param4,
                "",
                "Update",
                "Tanks"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteTank(int id)
        {
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                id.ToString(),
                "Delete",
                "Tanks"
            );
            return Ok(response);
        }

        // Tanks CRUD Ends Here..


        // Tanks refill CRUD Starts Here
        [Authorize]
        [HttpGet("view-tanks-refill-history")]
        public IActionResult ViewTanksRefillHisttory()
        {
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                "",
                "View",
                "TankRefills"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpGet("view-tank-refill-history/{id}")]
        public IActionResult ViewTankRefillHisttory(int id)
        {
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                id.ToString(),
                "View",
                "TankRefills"
            );
            return Ok(response);
        }


        [Authorize]
        [HttpPost("refill-tank")]
        public IActionResult RefillTank([FromBody] DBconnect dBconnect)
        {
            var response = _dbCall.CallStoredProcedure(
                dBconnect.param1,
                dBconnect.param2,
                dBconnect.param3,
                dBconnect.param4,
                "",
                "Insert",
                "TankRefills"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpPut("update-refill/{id}")]
        public IActionResult RefillTank(int id,[FromBody] DBconnect dBconnect)
        {
            var response = _dbCall.CallStoredProcedure(
                dBconnect.param1,
                dBconnect.param2,
                dBconnect.param3,
                dBconnect.param4,
                "",
                "Update",
                "TankRefills"
            );
            return Ok(response);
        }

    }
}
