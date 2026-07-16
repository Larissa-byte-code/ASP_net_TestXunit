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
            int nextId = (_db.TblSeller.Max(s => (int?)s.SellerId) ?? 0) + 1;
            string year = DateTime.Now.Year.ToString();

            seller.SellerId   = nextId;
            seller.SellerIdvC = $"Sel-{nextId:D3}-{year}";

            // Préfixe automatique pour Madagascar
            if (!string.IsNullOrEmpty(seller.SellerPhone) && !seller.SellerPhone.StartsWith("+261"))
            {
                seller.SellerPhone = $"+261{seller.SellerPhone}";
            }

            _db.TblSeller.Add(seller);
            await _db.SaveChangesAsync();
            return seller;
        }

        public async Task<TblSeller?> UpdateAsync(int id, TblSeller seller)
        {
            var existing = await _db.TblSeller.FindAsync(id);
            if (existing is null) return null;

            existing.SellerName  = seller.SellerName;
            existing.SellerAge   = seller.SellerAge;
            existing.SellerPhone = seller.SellerPhone.StartsWith("+261") 
                ? seller.SellerPhone 
                : $"+261{seller.SellerPhone}";
            existing.SellerPass  = seller.SellerPass;
            existing.Role        = seller.Role;

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
