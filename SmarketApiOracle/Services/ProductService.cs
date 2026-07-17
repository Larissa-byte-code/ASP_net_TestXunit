using Microsoft.EntityFrameworkCore;
using SmarketApiOracle.Data;
using SmarketApiOracle.Models;

namespace SmarketApiOracle.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _db;
        public ProductService(ApplicationDbContext db) => _db = db;

        public async Task<List<TblProduct>> GetAllAsync()
        {
            return await _db.TblProduct.ToListAsync();
        }

        public async Task<TblProduct?> GetByIdAsync(int id)
        {
            return await _db.TblProduct.FindAsync(id);
        }

        public async Task<TblProduct> AddAsync(TblProduct product)
        {
            // Étape 1 : insertion → Oracle génère prdId
            _db.TblProduct.Add(product);
            await _db.SaveChangesAsync();

            // Étape 2 : génération du code formaté
            product.prdIdvC = $"PR-{product.prdId:D3}-{DateTime.Now.Year}";

            // Étape 3 : sauvegarde directe
            await _db.SaveChangesAsync();

            return product;
        }

        public async Task<TblProduct?> UpdateAsync(int id, TblProduct product)
        {
            var existing = await _db.TblProduct.FindAsync(id);
            if (existing is null) return null;

            existing.prdName = product.prdName;
            existing.prdCat  = product.prdCat;

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _db.TblProduct.FindAsync(id);
            if (product is null) return false;

            _db.TblProduct.Remove(product);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
