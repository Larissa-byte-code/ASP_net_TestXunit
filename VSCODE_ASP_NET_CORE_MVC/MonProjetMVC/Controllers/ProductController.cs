using Microsoft.AspNetCore.Mvc;
using MonProjetMVC.Data;
using MonProjetMVC.Models;

namespace MonProjetMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTE
        public IActionResult Index()
        {
            var products = _context.TblProduct.ToList();
            int nextId = _context.TblProduct.Any()
                ? _context.TblProduct.Max(p => p.prdId) + 1
                : 1;
            ViewBag.NextId   = nextId;
            ViewBag.NextCode = $"Prd-{nextId:D3}-{DateTime.Now.Year}";
            return View(products);
        }

        // NEW
        public IActionResult New()
        {
            int nextId = _context.TblProduct.Any()
                ? _context.TblProduct.Max(p => p.prdId) + 1
                : 1;
            ViewBag.NextId   = nextId;
            ViewBag.NextCode = $"Prd-{nextId:D3}-{DateTime.Now.Year}";
            return View("Index", _context.TblProduct.ToList());
        }

        // CREATE POST
        [HttpPost]
        public IActionResult Create(TblProduct product)
        {
            ModelState.Remove("prdId");
            ModelState.Remove("prdIdvC");

            if (ModelState.IsValid)
            {
                int nextId = _context.TblProduct.Any()
                    ? _context.TblProduct.Max(p => p.prdId) + 1
                    : 1;

                product.prdId   = nextId;
                product.prdIdvC = $"Prd-{nextId:D3}-{DateTime.Now.Year}";

                _context.TblProduct.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            TempData["Debug"] = string.Join(" | ",
                ModelState
                    .Where(x => x.Value!.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {x.Value!.Errors.First().ErrorMessage}")
            );

            int nId = _context.TblProduct.Any()
                ? _context.TblProduct.Max(p => p.prdId) + 1
                : 1;
            ViewBag.NextId   = nId;
            ViewBag.NextCode = $"Prd-{nId:D3}-{DateTime.Now.Year}";
            return View("Index", _context.TblProduct.ToList());
        }

        // EDIT GET
        public IActionResult Edit(int id)
        {
            var product = _context.TblProduct.Find(id);
            if (product == null) return NotFound();
            ViewBag.EditProduct = product;
            return View("Index", _context.TblProduct.ToList());
        }

        // EDIT POST
        [HttpPost]
        [ActionName("Edit")]
        public IActionResult EditPost(TblProduct product)
        {
            ModelState.Remove("prdId");
            ModelState.Remove("prdIdvC");

            if (ModelState.IsValid)
            {
                var existing = _context.TblProduct
                    .FirstOrDefault(p => p.prdId == product.prdId);

                if (existing != null)
                {
                    existing.prdName = product.prdName;
                    existing.prdCat  = product.prdCat;
                    _context.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            TempData["Debug"] = string.Join(" | ",
                ModelState
                    .Where(x => x.Value!.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {x.Value!.Errors.First().ErrorMessage}")
            );

            ViewBag.EditProduct = product;
            return View("Index", _context.TblProduct.ToList());
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            var product = _context.TblProduct.Find(id);
            if (product != null)
            {
                _context.TblProduct.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}