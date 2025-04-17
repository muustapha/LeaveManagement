using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;

namespace LeaveManagement.Controllers
{
    [ApiController]
    [Route("api/authentification")]
    public class AuthentificationController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        public AuthentificationController(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Login([FromBody] string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return BadRequest("Le rôle est requis pour la connexion.");

            var validRoles = new[] { "Employee", "Manager" };
            if (!validRoles.Contains(role))
                return BadRequest("Rôle invalide. Les rôles valides sont : Employee, Manager.");

            var key   = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "DemoUser"),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                claims:             claims,
                expires:            DateTime.UtcNow.AddHours(_jwtSettings.TokenExpiryInHours),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { token = jwt });
        }
    }
}
