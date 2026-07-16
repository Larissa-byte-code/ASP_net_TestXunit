using Microsoft.AspNetCore.Mvc;
using SmarketApiOracle.Services;

namespace SmarketApiOracle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _service;

        public AuthController(AuthService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public IActionResult Register(string username, string email, string password, string role = "User")
        {
            var result = _service.Register(username, email, password, role);
            return Ok(result);
        }

        [HttpPost("login")]
        public IActionResult Login(string email, string password)
        {
            var token = _service.Login(email, password);
            return token == null ? Unauthorized() : Ok(new { token });
        }
    }
}
