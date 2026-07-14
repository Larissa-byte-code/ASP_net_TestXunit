using Microsoft.AspNetCore.Mvc;
using SmarketApi.Data;
using SmarketApi.Models;

namespace SmarketApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SellingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SellingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /api/selling/all
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var ventes = _context.TblSelling.ToList();
            return Ok(ventes);
        }

        // GET /api/selling/get/{id}
        [HttpGet("get/{id}")]
        public IActionResult GetById(int id)
        {
            var vente = _context.TblSelling.Find(id);
            if (vente == null) return NotFound();

            var details = _context.TblDetailSelling
                .Where(d => d.VenteId == id)
                .ToList();

            return Ok(new { vente, details });
        }

        // POST /api/selling/add
        [HttpPost("add")]
        public IActionResult Create([FromBody] VenteIndexViewModel model)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                model.Selling.VenteId = 0;
                model.Selling.DateVente = model.Selling.DateVente == default
                    ? DateTime.Now
                    : model.Selling.DateVente;

                model.Selling.TotalAmount = model.Details
                    .Where(d => d.ProductId != 0 && d.Qty > 0)
                    .Sum(d => d.Qty * d.Price);

                model.Selling.Numfacture = "TEMP";
                _context.TblSelling.Add(model.Selling);
                _context.SaveChanges();

                int venteId = model.Selling.VenteId;
                string year = DateTime.Now.Year.ToString();
                model.Selling.Numfacture = $"Facture-{venteId:D3}-{year}";
                _context.SaveChanges();

                foreach (var detail in model.Details)
                {
                    if (detail.ProductId == 0 || detail.Qty <= 0) continue;

                    detail.VenteId   = venteId;
                    detail.LineTotal = detail.Qty * detail.Price;
                    _context.TblDetailSelling.Add(detail);

                    string productIdStr = detail.ProductId.ToString();
                    var stock = _context.TblStock.FirstOrDefault(s => s.ProductId == productIdStr);

                    if (stock != null && stock.QtyDisponible >= detail.Qty)
                    {
                        stock.QtyDisponible -= detail.Qty;
                        stock.DateMaj = DateTime.Now;
                    }

                    var mouvement = new TblStockMouvement
                    {
                        MouvementId   = Guid.NewGuid().ToString().Substring(0, 10),
                        ProductId     = productIdStr,
                        TypeMouvement = "OUT",
                        Qty           = detail.Qty,
                        DateMouvement = DateTime.Now,
                        RefVenteId    = venteId.ToString(),
                        Commentaire   = "Sortie de stock / Vente"
                    };
                    _context.TblStockMouvement.Add(mouvement);
                }

                _context.SaveChanges();
                transaction.Commit();

                return CreatedAtAction(nameof(GetById), new { id = venteId }, model.Selling);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return BadRequest(new { error = ex.Message });
            }
        }

        // DELETE /api/selling/delete/{id}
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var vente = _context.TblSelling.Find(id);
            if (vente == null) return NotFound();

            var details = _context.TblDetailSelling.Where(d => d.VenteId == id).ToList();
            _context.TblDetailSelling.RemoveRange(details);

            var mouvements = _context.TblStockMouvement.Where(m => m.RefVenteId == id.ToString()).ToList();
            _context.TblStockMouvement.RemoveRange(mouvements);

            _context.TblSelling.Remove(vente);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
