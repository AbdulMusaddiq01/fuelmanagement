using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FuelmanagementApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace FuelmanagementApi.Services
{
    public class GenerateJwtToken
    {
        public readonly IConfiguration _configuration;
        public GenerateJwtToken(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(string user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
            var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires:DateTime.Now.AddDays(10),
                signingCredentials:credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
