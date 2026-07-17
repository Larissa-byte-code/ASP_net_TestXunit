using Microsoft.EntityFrameworkCore;
using SmarketApiOracle.Data;
using SmarketApiOracle.Models;

namespace SmarketApiOracle.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _db;
        public CategoryService(ApplicationDbContext db) => _db = db;

        // Récupérer toutes les catégories
        public async Task<List<TblCategory>> GetAllAsync()
        {
            return await _db.TblCategory.ToListAsync();
        }

        // Récupérer une catégorie par ID
        public async Task<TblCategory?> GetByIdAsync(int id)
        {
            return await _db.TblCategory.FindAsync(id);
        }

        // Ajouter une nouvelle catégorie
        public async Task<TblCategory> AddAsync(TblCategory category)
        {
            // Étape 1 : insertion → Oracle génère CatId
            _db.TblCategory.Add(category);
            await _db.SaveChangesAsync();

            // Étape 2 : CatId est connu, on génère CatIdvC
            category.CatIdvC = $"Cat-{category.CatId:D3}-{DateTime.Now.Year}";

            // Étape 3 : sauvegarde directe (EF suit déjà l’objet)
            await _db.SaveChangesAsync();

            return category;
        }

        // Mettre à jour une catégorie
        public async Task<TblCategory?> UpdateAsync(int id, TblCategory category)
        {
            var existing = await _db.TblCategory.FindAsync(id);
            if (existing is null) return null;

            existing.CatName = category.CatName;
            existing.CatDes  = category.CatDes;
            await _db.SaveChangesAsync();
            return existing;
        }

        // Supprimer une catégorie
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
