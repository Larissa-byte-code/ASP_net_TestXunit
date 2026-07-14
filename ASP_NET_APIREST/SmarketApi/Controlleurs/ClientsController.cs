using Microsoft.AspNetCore.Mvc;
using SmarketApi.Data;
using SmarketApi.Models;

namespace SmarketApi.Controllers
{
    // [ApiController] → indique que ce contrôleur est une API REST
    // [Route("api/[controller]")] → les endpoints seront accessibles via /api/clients
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /api/clients/all
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var clients = _context.TblClient.ToList();
            return Ok(clients);
        }

        // GET /api/clients/get/{id}
        [HttpGet("get/{id}")]
        public IActionResult GetById(int id)
        {
            var client = _context.TblClient.Find(id);
            if (client == null) return NotFound();
            return Ok(client);
        }

        // POST /api/clients/add
        [HttpPost("add")]
        public IActionResult Create([FromBody] TblClient client)
        {
            int nextId = _context.TblClient.Any()
                ? _context.TblClient.Max(c => c.ClientId) + 1
                : 1;

            client.ClientId    = nextId;
            client.ClientCode  = $"CL-{nextId:D3}-{DateTime.Now.Year}";
            client.DateCreated = DateTime.Now;
            client.IsActive    = client.IsActive;

            _context.TblClient.Add(client);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = client.ClientId }, client);
        }

        // PUT /api/clients/update/{id}
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] TblClient client)
        {
            var existing = _context.TblClient.Find(id);
            if (existing == null) return NotFound();

            existing.ClientName    = client.ClientName;
            existing.ClientPhone   = client.ClientPhone;
            existing.ClientEmail   = client.ClientEmail;
            existing.ClientAddress = client.ClientAddress;
            existing.IsActive      = client.IsActive;

            _context.SaveChanges();
            return Ok(existing);
        }

        // DELETE /api/clients/delete/{id}
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var client = _context.TblClient.Find(id);
            if (client == null) return NotFound();

            _context.TblClient.Remove(client);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
