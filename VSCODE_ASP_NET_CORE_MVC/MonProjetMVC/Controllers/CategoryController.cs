using Microsoft.AspNetCore.Mvc;
using MonProjetMVC.Data;
using MonProjetMVC.Models;

namespace MonProjetMVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTE
        /*
            // Déclare une variable entière qui va contenir le prochain ID
            int nextId;

            // Vérifie si la table TblCategory contient au moins une ligne
            if (_context.TblCategory.Any())
            {
                // Si oui :
                // - on prend la valeur maximale de la colonne CatId
                // - on ajoute 1 pour obtenir le prochain identifiant disponible
                nextId = _context.TblCategory.Max(c => c.CatId) + 1;
            }
            else
            {
                // Si la table est vide :
                // - on commence à 1 (premier identifiant)
                nextId = 1;
            }
        */

        public IActionResult Index()
        {
            //Va chercher toutes les lignes.
            //SELECT * FROM TblCategory
            var categories = _context.TblCategory.ToList();
            //Est-ce que la table contient au moins une ligne ?
            int nextId = _context.TblCategory.Any()
                ? _context.TblCategory.Max(c => c.CatId) + 1
                : 1;
                //ViewBag → objet dynamique pour envoyer des données à la vue
            ViewBag.NextId   = nextId;
            ViewBag.NextCode = $"Cat-{nextId:D3}-{DateTime.Now.Year}";
            //Cherche automatiquement /Views/Category/Index.cshtml et lui passe categories comme modèle (@model IEnumerable<TblCategory>).
            return View(categories);
        }

        // NEW
        public IActionResult New()
        {
            int nextId = _context.TblCategory.Any()
                ? _context.TblCategory.Max(c => c.CatId) + 1
                : 1;
                //ViewBag permet d'envoyer une donnée à la Vue. RAZOR
            ViewBag.NextId   = nextId;
            ViewBag.NextCode = $"Cat-{nextId:D3}-{DateTime.Now.Year}";
            var categories = _context.TblCategory.ToList();
            return View("Index", categories);
        }

        // CREATE POST
        [HttpPost]
        public IActionResult Create(TblCategory category)
        {
            //ModelState → objet qui contient la validation du modèle.
            //.Remove("CatId") → on enlève la validation automatique sur la clé primaire (car elle est générée par le code, pas par l’utilisateur).
            ModelState.Remove("CatId");
            ModelState.Remove("CatIdvC");
             /*
            ModelState vient de la classe Controller — il contient le résultat de la validation automatique 
            de tous les champs reçus. Comme CatId et CatIdvC sont générés côté serveur
            (pas saisis par l'utilisateur), on les retire de la validation pour éviter des erreurs.
            */
            //si tous les champs requis sont bien remplis et valides
            if (ModelState.IsValid)
            {
                int nextId = _context.TblCategory.Any()
                    ? _context.TblCategory.Max(c => c.CatId) + 1
                    : 1;

                category.CatId   = nextId;
                category.CatIdvC = $"Cat-{nextId:D3}-{DateTime.Now.Year}";

                _context.TblCategory.Add(category);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            TempData["Debug"] = string.Join(" | ",
                ModelState
                    .Where(x => x.Value!.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {x.Value!.Errors.First().ErrorMessage}")
            );

            int nId = _context.TblCategory.Any()
                ? _context.TblCategory.Max(c => c.CatId) + 1
                : 1;
            ViewBag.NextId   = nId;
            ViewBag.NextCode = $"Cat-{nId:D3}-{DateTime.Now.Year}";
            return View("Index", _context.TblCategory.ToList());
        }

        // EDIT GET
        public IActionResult Edit(int id)
        {
            var category = _context.TblCategory.Find(id);
            if (category == null) return NotFound();
            ViewBag.EditCategory = category;
            return View("Index", _context.TblCategory.ToList());
        }

        // EDIT POST
        [HttpPost]
        [ActionName("Edit")]
        public IActionResult EditPost(TblCategory category)
        {
            ModelState.Remove("CatId");
            ModelState.Remove("CatIdvC");

            if (ModelState.IsValid)
            {
                /*
                On cherche l'enregistrement existant en base, 
                on modifie seulement les champs voulus, puis SaveChanges()
                 exécute le UPDATE. On ne touche pas à CatId ni CatIdvC 
                 pour ne pas les écraser.
                */
                var existing = _context.TblCategory
                    .FirstOrDefault(c => c.CatId == category.CatId);

                if (existing != null)
                {
                    existing.CatName = category.CatName;
                    existing.CatDes  = category.CatDes;
                    _context.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            TempData["Debug"] = string.Join(" | ",
                ModelState
                    .Where(x => x.Value!.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {x.Value!.Errors.First().ErrorMessage}")
            );

            ViewBag.EditCategory = category;
            return View("Index", _context.TblCategory.ToList());
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            var category = _context.TblCategory.Find(id);
            if (category != null)
            {
                _context.TblCategory.Remove(category);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
/*
Résumé des objets "magiques" fournis par Controller :
Objet       	D'où ça vient                   A quoi ça sert  
ViewBag   	 Classe Controller          Envoyer des données à la vue, sans modèle fort
TempData  	 Classe Controller  	    Envoyer des données qui survivent à 1 redirect
ModelState 	 Classe Controller          Résultat de la validation des champs du formulaire
_context   	 Injection de dépendances   Accès à la base de données
                   (Program.cs)
View()            Classe Controller          Retourner une vue HTML
RedirectToAction()Classe Controller          Rediriger vers une autre action
NotFound()        Classe Controller          Retourner une erreur HTTP 404
*/