using Microsoft.AspNetCore.Mvc;
using SmarketApi.Data;
using SmarketApi.Models;

namespace SmarketApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SellersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SellersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /api/sellers/all
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var sellers = _context.TblSeller.ToList();
            return Ok(sellers);
        }

        // GET /api/sellers/get/{id}
        [HttpGet("get/{id}")]
        public IActionResult GetById(int id)
        {
            var seller = _context.TblSeller.Find(id);
            if (seller == null) return NotFound();
            return Ok(seller);
        }

        // POST /api/sellers/add
        [HttpPost("add")]
        public IActionResult Create([FromBody] TblSeller seller)
        {
            int nextId = (_context.TblSeller.Max(s => (int?)s.SellerId) ?? 0) + 1;
            string year = DateTime.Now.Year.ToString();

            seller.SellerId   = nextId;
            seller.SellerIdvC = $"Sel-{nextId:D3}-{year}";

            // Préfixe automatique pour Madagascar
            if (!string.IsNullOrEmpty(seller.SellerPhone) && !seller.SellerPhone.StartsWith("+261"))
            {
                seller.SellerPhone = $"+261{seller.SellerPhone}";
            }

            _context.TblSeller.Add(seller);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = seller.SellerId }, seller);
        }

        // PUT /api/sellers/update/{id}
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] TblSeller seller)
        {
            var existing = _context.TblSeller.Find(id);
            if (existing == null) return NotFound();

            existing.SellerName  = seller.SellerName;
            existing.SellerAge   = seller.SellerAge;
            existing.SellerPhone = seller.SellerPhone.StartsWith("+261") 
                ? seller.SellerPhone 
                : $"+261{seller.SellerPhone}";
            existing.SellerPass  = seller.SellerPass;
            existing.Role        = seller.Role;

            _context.SaveChanges();
            return Ok(existing);
        }

        // DELETE /api/sellers/delete/{id}
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var seller = _context.TblSeller.Find(id);
            if (seller == null) return NotFound();

            _context.TblSeller.Remove(seller);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
