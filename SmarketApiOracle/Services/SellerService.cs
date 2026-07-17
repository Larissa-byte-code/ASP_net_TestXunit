using Microsoft.EntityFrameworkCore;
using SmarketApiOracle.Data;
using SmarketApiOracle.Models;

namespace SmarketApiOracle.Services
{
    public class SellerService
    {
        private readonly ApplicationDbContext _db;
        public SellerService(ApplicationDbContext db) => _db = db;

        public async Task<List<TblSeller>> GetAllAsync()
        {
            return await _db.TblSeller.ToListAsync();
        }

        public async Task<TblSeller?> GetByIdAsync(int id)
        {
            return await _db.TblSeller.FindAsync(id);
        }

        public async Task<TblSeller> AddAsync(TblSeller seller)
        {
            // Étape 1 : insertion → Oracle génère SellerId
            _db.TblSeller.Add(seller);
            await _db.SaveChangesAsync();

            // Étape 2 : génération du code formaté
            seller.SellerIdvC = $"Sel-{seller.SellerId:D3}-{DateTime.Now.Year}";

            // Étape 3 : préfixe automatique pour Madagascar
            if (!string.IsNullOrEmpty(seller.SellerPhone) && !seller.SellerPhone.StartsWith("+261"))
            {
                seller.SellerPhone = $"+261{seller.SellerPhone}";
            }

            // Étape 4 : sauvegarde directe
            await _db.SaveChangesAsync();

            return seller;
        }

        public async Task<TblSeller?> UpdateAsync(int id, TblSeller seller)
        {
            var existing = await _db.TblSeller.FindAsync(id);
            if (existing is null) return null;

            existing.SellerName  = seller.SellerName;
            existing.SellerAge   = seller.SellerAge;
            existing.SellerPass  = seller.SellerPass;
            existing.Role        = seller.Role;

            // Préfixe automatique pour Madagascar
            existing.SellerPhone = seller.SellerPhone.StartsWith("+261")
                ? seller.SellerPhone
                : $"+261{seller.SellerPhone}";

            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var seller = await _db.TblSeller.FindAsync(id);
            if (seller is null) return false;

            _db.TblSeller.Remove(seller);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
