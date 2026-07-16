using Microsoft.EntityFrameworkCore;
using SmarketApiOracle.Data;
using SmarketApiOracle.Models;

namespace SmarketApiOracle.Services
{
    public class SellingService
    {
        private readonly ApplicationDbContext _db;
        public SellingService(ApplicationDbContext db) => _db = db;

        public async Task<List<TblSelling>> GetAllAsync()
        {
            return await _db.TblSelling.ToListAsync();
        }

        public async Task<object?> GetByIdAsync(int id)
        {
            var vente = await _db.TblSelling.FindAsync(id);
            if (vente is null) return null;

            var details = await _db.TblDetailSelling
                .Where(d => d.VenteId == id)
                .ToListAsync();

            return new { vente, details };
        }

        public async Task<TblSelling> AddAsync(VenteIndexViewModel model)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
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
                _db.TblSelling.Add(model.Selling);
                await _db.SaveChangesAsync();

                int venteId = model.Selling.VenteId;
                string year = DateTime.Now.Year.ToString();
                model.Selling.Numfacture = $"Facture-{venteId:D3}-{year}";
                await _db.SaveChangesAsync();

                foreach (var detail in model.Details)
                {
                    if (detail.ProductId == 0 || detail.Qty <= 0) continue;

                    detail.VenteId   = venteId;
                    detail.LineTotal = detail.Qty * detail.Price;
                    _db.TblDetailSelling.Add(detail);

                    string productIdStr = detail.ProductId.ToString();
                    var stock = _db.TblStock.FirstOrDefault(s => s.ProductId == productIdStr);

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
                    _db.TblStockMouvement.Add(mouvement);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return model.Selling;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vente = await _db.TblSelling.FindAsync(id);
            if (vente is null) return false;

            var details = _db.TblDetailSelling.Where(d => d.VenteId == id).ToList();
            _db.TblDetailSelling.RemoveRange(details);

            var mouvements = _db.TblStockMouvement.Where(m => m.RefVenteId == id.ToString()).ToList();
            _db.TblStockMouvement.RemoveRange(mouvements);

            _db.TblSelling.Remove(vente);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
