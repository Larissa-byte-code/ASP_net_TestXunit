using Microsoft.AspNetCore.Mvc;
using SmarketApiOracle.Models;
using SmarketApiOracle.Services;

namespace SmarketApiOracle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SellingController : ControllerBase
    {
        private readonly SellingService _service;

        public SellingController(SellingService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var ventes = await _service.GetAllAsync();
            return Ok(ventes);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var vente = await _service.GetByIdAsync(id);
            return vente is null ? NotFound() : Ok(vente);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create([FromBody] VenteIndexViewModel model)
        {
            try
            {
                var created = await _service.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = created.VenteId }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
