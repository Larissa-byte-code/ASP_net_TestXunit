using Microsoft.AspNetCore.Mvc;
using SmarketApiOracle.Models;
using SmarketApiOracle.Services;

namespace SmarketApiOracle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ClientService _service;

        public ClientsController(ClientService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var clients = await _service.GetAllAsync();
            return Ok(clients);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var client = await _service.GetByIdAsync(id);
            return client is null ? NotFound() : Ok(client);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create([FromBody] TblClient client)
        {
            var created = await _service.AddAsync(client);
            return CreatedAtAction(nameof(GetById), new { id = created.ClientId }, created);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TblClient client)
        {
            var updated = await _service.UpdateAsync(id, client);
            return updated is null ? NotFound() : Ok(updated);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
