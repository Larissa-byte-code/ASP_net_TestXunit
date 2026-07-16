using Microsoft.EntityFrameworkCore;
using SmarketApiOracle.Data;
using SmarketApiOracle.Models;

namespace SmarketApiOracle.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _db;
        public CategoryService(ApplicationDbContext db) => _db = db;

        public async Task<List<TblCategory>> GetAllAsync()
        {
            return await _db.TblCategory.ToListAsync();
        }

        public async Task<TblCategory?> GetByIdAsync(int id)
        {
            return await _db.TblCategory.FindAsync(id);
        }

        public async Task<TblCategory> AddAsync(TblCategory category)
        {
            //Vérifie si la table TblCategory contient au moins une ligne.
            int nextId = _db.TblCategory.Any()
                ? _db.TblCategory.Max(c => c.CatId) + 1
                : 1;

            category.CatId   = nextId;
            category.CatIdvC = $"Cat-{nextId:D3}-{DateTime.Now.Year}";

            _db.TblCategory.Add(category);
            await _db.SaveChangesAsync();
            return category;
        }

        public async Task<TblCategory?> UpdateAsync(int id, TblCategory category)
        {
            var existing = await _db.TblCategory.FindAsync(id);
            if (existing is null) return null;

            existing.CatName = category.CatName;
            existing.CatDes  = category.CatDes;
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _db.TblCategory.FindAsync(id);
            if (category is null) return false;

            _db.TblCategory.Remove(category);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
