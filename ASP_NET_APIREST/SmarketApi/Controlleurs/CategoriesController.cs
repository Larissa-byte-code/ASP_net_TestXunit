using Microsoft.AspNetCore.Mvc;
using SmarketApi.Data;
using SmarketApi.Models;

namespace SmarketApi.Controllers
{
    // [ApiController] → indique que ce contrôleur est une API REST
    // [Route("api/[controller]")] → les endpoints seront accessibles via /api/categories
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        // Endpoint qui retourne la liste de toutes les catégories en JSON
       // GET /api/categories/all
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var categories = _context.TblCategory.ToList();
            return Ok(categories);
        }

        
        //  Endpoint qui retourne une seule catégorie par son identifiant
        // GET /api/categories/{id}
        [HttpGet("get/{id}")]
        public IActionResult GetById(int id)
        {
            var category = _context.TblCategory.Find(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        
        // Endpoint qui ajoute une nouvelle catégorie dans la base
        // POST /api/categories/add
        [HttpPost("add")]
        public IActionResult Create(TblCategory category)
        {
            // Génère automatiquement CatId et CatIdvC comme dans ton MVC
            int nextId = _context.TblCategory.Any()
                ? _context.TblCategory.Max(c => c.CatId) + 1
                : 1;

            category.CatId   = nextId;
            category.CatIdvC = $"Cat-{nextId:D3}-{DateTime.Now.Year}";

            _context.TblCategory.Add(category);
            _context.SaveChanges();

            // Retourne la catégorie créée avec son nouvel ID
            return CreatedAtAction(nameof(GetById), new { id = category.CatId }, category);
        }

       
        // Endpoint qui modifie une catégorie existante
       // PUT /api/categories/update/{id}
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, TblCategory category)
        {
            //Si elle existe, on la stocke dans existing.

            //Sinon, existing sera null.
            var existing = _context.TblCategory.Find(id);
            if (existing == null) return NotFound();

            // Met à jour uniquement les champs modifiables
            existing.CatName = category.CatName;
            existing.CatDes  = category.CatDes;
            _context.SaveChanges();

            return Ok(existing);
        }

        
        // Endpoint qui supprime une catégorie par son identifiant
        // DELETE /api/categories/delete/{id}
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var category = _context.TblCategory.Find(id);
            if (category == null) return NotFound();

            _context.TblCategory.Remove(category);
            _context.SaveChanges();

            return NoContent(); 
        }
    }
}
