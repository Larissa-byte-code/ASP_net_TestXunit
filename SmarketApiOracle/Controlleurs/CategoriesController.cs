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

                
            //
            [HttpGet("all")]
            public async Task<IActionResult> GetAll()
            {
                var categories = await _service.GetAllAsync();
                return Ok(categories); 
            }

            //
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _service.GetByIdAsync(id);
            return category is null ? NotFound() : Ok(category);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create([FromBody] TblCategory category)
        {
            try
            {
                var created = await _service.AddAsync(category);
                // Retourne 201 Created avec l’objet JSON
                return CreatedAtAction(nameof(GetById), new { id = created.CatId }, created);
            }
            catch (Exception ex)
            {
                // Toujours renvoyer du JSON même en cas d’erreur
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TblCategory category)
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
