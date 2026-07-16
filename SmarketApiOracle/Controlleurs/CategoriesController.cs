using Microsoft.AspNetCore.Mvc;
using SmarketApiOracle.Models;
using SmarketApiOracle.Services;

namespace SmarketApiOracle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _service;

        public CategoriesController(CategoryService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _service.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _service.GetByIdAsync(id);
            return category is null ? NotFound() : Ok(category);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create(TblCategory category)
        {
            var created = await _service.AddAsync(category);
            return CreatedAtAction(nameof(GetById), new { id = created.CatId }, created);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, TblCategory category)
        {
            var updated = await _service.UpdateAsync(id, category);
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
