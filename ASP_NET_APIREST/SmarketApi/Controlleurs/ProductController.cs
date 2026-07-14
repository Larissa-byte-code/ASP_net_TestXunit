using Microsoft.AspNetCore.Mvc;
using SmarketApi.Data;
using SmarketApi.Models;

namespace SmarketApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /api/products/all
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var products = _context.TblProduct.ToList();
            return Ok(products);
        }

        // GET /api/products/get/{id}
        [HttpGet("get/{id}")]
        public IActionResult GetById(int id)
        {
            var product = _context.TblProduct.Find(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // POST /api/products/add
        [HttpPost("add")]
        public IActionResult Create([FromBody] TblProduct product)
        {
            int nextId = _context.TblProduct.Any()
                ? _context.TblProduct.Max(p => p.prdId) + 1
                : 1;

            product.prdId   = nextId;
            product.prdIdvC = $"Prd-{nextId:D3}-{DateTime.Now.Year}";

            _context.TblProduct.Add(product);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = product.prdId }, product);
        }

        // PUT /api/products/update/{id}
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] TblProduct product)
        {
            var existing = _context.TblProduct.Find(id);
            if (existing == null) return NotFound();

            existing.prdName = product.prdName;
            existing.prdCat  = product.prdCat;

            _context.SaveChanges();
            return Ok(existing);
        }

        // DELETE /api/products/delete/{id}
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var product = _context.TblProduct.Find(id);
            if (product == null) return NotFound();

            _context.TblProduct.Remove(product);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
