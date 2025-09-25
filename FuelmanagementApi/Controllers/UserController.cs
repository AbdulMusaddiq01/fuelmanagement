using FuelmanagementApi.Models;
using FuelmanagementApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FuelmanagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly PrepareRequest _prepareRequest;
        private readonly GenerateJwtToken _generateJwtToken;
        private readonly DbCall _dbCall;
        public UserController(PrepareRequest prepareRequest, DbCall dbCall, GenerateJwtToken generateJwtToken)
        {
            _prepareRequest = prepareRequest;
            _dbCall = dbCall;
            _generateJwtToken = generateJwtToken;
        }

        //[AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Index([FromBody] DBconnect dBconnect)
        {
            var response = _dbCall.CallStoredProcedure(
                dBconnect.param1,
                dBconnect.param2,
                dBconnect.param3,
                dBconnect.param4,
                "",
                "Login",
                "LoginUsers"
            );

            var tokenstring = _generateJwtToken.GenerateToken("Musaddiq");

            return Ok(new
            {
                Data = response,
                Token = tokenstring
            });
        }

        [Authorize]
        [HttpPost("create")]
        public IActionResult Create([FromBody] DBconnect dBconnect)
        {
            var response = _dbCall.CallStoredProcedure(
                dBconnect.param1,
                dBconnect.param2,
                dBconnect.param3,
                dBconnect.param4,
                "",
                dBconnect.operation,
                "LoginUsers"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpGet("view")]
        public IActionResult ViewUsers()
        {
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                "",
                "View",
                "LoginUsers"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpGet("view/{id}")]
        public IActionResult ViewUser(int id)
        {
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "", 
                id.ToString(),
                "View",
                "LoginUsers"
            );
            return Ok(response);
        }


       

        [Authorize]
        [HttpPut("update/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] DBconnect dBconnect)
        {
            var response = _dbCall.CallStoredProcedure(
                dBconnect.param1,
                dBconnect.param2,
                dBconnect.param3,
                dBconnect.param4,
                "",
                "Update",
                "LoginUsers"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteUser(int id) {
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                id.ToString(),
                "Delete",
                "LoginUsers"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpGet("issuers")]
        public IActionResult GetAllIssuers() { 
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                "",
                "View",
                "Issuers"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpGet("issuer/{id}")]
        public IActionResult GetIssuer(int id)
        {
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                id.ToString(),
                "View",
                "Issuers"
            );
            return Ok(response);
        }


        [Authorize]
        [HttpPost("create-issuer")]
        public IActionResult UpdateIssuer([FromBody] DBconnect dBconnect)
        {
            var response = _dbCall.CallStoredProcedure(
                dBconnect.param1,
                dBconnect.param2,
                dBconnect.param3,
                dBconnect.param4,
                "",
                "Insert",
                "Issuers"
            );
            return Ok(response);
        }


        [Authorize]
        [HttpPut("update-issuer/{id}")]
        public IActionResult UpdateIssuer(int id, [FromBody] DBconnect dBconnect)
        {
            var response = _dbCall.CallStoredProcedure(
                dBconnect.param1,
                dBconnect.param2,
                dBconnect.param3,
                dBconnect.param4,
                "",
                "Update",
                "Issuers"
            );
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("delete-issuer/{id}")]
        public IActionResult DeleteIssuer(int id) {
            var response = _dbCall.CallStoredProcedure(
                "",
                "",
                "",
                "",
                id.ToString(),
                "Delete",
                "Issuers"
            );
            return Ok(response);
        }
    }
}
