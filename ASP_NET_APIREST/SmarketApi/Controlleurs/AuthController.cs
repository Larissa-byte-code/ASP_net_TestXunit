using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SmarketApi.Data;
using SmarketApi.Models;
using BCrypt.Net;

namespace SmarketApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Endpoint d'inscription
        [HttpPost("register")]
        public IActionResult Register(string username, string email, string password, string role = "User")
        {
            var user = new TblUser
            {
                UserName = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role
            };

            _context.TblUsers.Add(user);
            _context.SaveChanges();

            return Ok("Utilisateur créé !");
        }

        // Endpoint de connexion
        [HttpPost("login")]
        public IActionResult Login(string email, string password)
        {
            var user = _context.TblUsers.SingleOrDefault(u => u.Email == email);
            if (user == null) return Unauthorized();

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty)

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MaCléSuperSecrète12345"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "SmarketApi",
                audience: "SmarketApiUsers",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
