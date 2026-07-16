using Microsoft.AspNetCore.Mvc;
using SmarketApiOracle.Models;
using SmarketApiOracle.Services;

namespace SmarketApiOracle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SellersController : ControllerBase
    {
        private readonly SellerService _service;

        public SellersController(SellerService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var sellers = await _service.GetAllAsync();
            return Ok(sellers);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var seller = await _service.GetByIdAsync(id);
            return seller is null ? NotFound() : Ok(seller);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create([FromBody] TblSeller seller)
        {
            var created = await _service.AddAsync(seller);
            return CreatedAtAction(nameof(GetById), new { id = created.SellerId }, created);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TblSeller seller)
        {
            var updated = await _service.UpdateAsync(id, seller);
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
