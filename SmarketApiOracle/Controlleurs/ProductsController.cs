using Microsoft.AspNetCore.Mvc;
using SmarketApiOracle.Models;
using SmarketApiOracle.Services;

namespace SmarketApiOracle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductsController(ProductService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);
            return product is null ? NotFound() : Ok(product);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create([FromBody] TblProduct product)
        {
            var created = await _service.AddAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = created.prdId }, created);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TblProduct product)
        {
            var updated = await _service.UpdateAsync(id, product);
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
