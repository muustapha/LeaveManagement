using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LeaveManagement.Controllers
{
    [ApiController]
    [Route("api/authentification")]
    public class AuthentificationController : ControllerBase
    {
        // Clé secrète (à extraire en configuration en production)
        private const string SecretKey = "this_is_a_super_secret_key_for_demo_only";

        /// <summary>
        /// Point de terminaison de connexion pour obtenir un JWT.
        /// Envoie le role souhaité dans le corps (e.g. "Employee" ou "Manager").
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return BadRequest("Le rôle est requis pour la connexion.");

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "DemoUser"),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { token = jwt });
        }
    }
}
