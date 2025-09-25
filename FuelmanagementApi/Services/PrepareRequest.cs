using System.Text.Json;
using FuelmanagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FuelmanagementApi.Services
{
    public class PrepareRequest
    {
        private readonly DbCall _dbCall;

        public PrepareRequest(DbCall dbCall)
        {
            _dbCall = dbCall;
        }

        public List<Dictionary<string,object>> PrepareReq(DBconnect dbConnect)
        {
            return _dbCall.CallStoredProcedure(
                dbConnect.param1,
                dbConnect.param2,
                dbConnect.param3,
                dbConnect.param4,
                dbConnect.search,
                dbConnect.operation,
                dbConnect.screen
           );
        }

    }
}
