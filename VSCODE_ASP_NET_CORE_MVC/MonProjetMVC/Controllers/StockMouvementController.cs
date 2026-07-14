using Microsoft.AspNetCore.Mvc;
using MonProjetMVC.Data;
using MonProjetMVC.Models;
using MonProjetMVC.ViewModels;

namespace MonProjetMVC.Controllers
{
    public class StockMouvementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockMouvementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // INDEX : affiche stocks + mouvements
        public IActionResult Index()
        {
            ChargerListes();

            string nextId = GetNextStockId();
            ViewBag.NextStockId = nextId;
            ViewBag.NextCode    = $"{nextId}-{DateTime.Now.Year}";

            var vm = new StockPageViewModel
            {
                Stocks     = _context.TblStock.ToList(),
                Mouvements = _context.TblStockMouvement.ToList()
            };

            return View(vm);
        }

        // NEW : reset formulaire
        public IActionResult New()
            {
                string nextId = GetNextStockId();
                ViewBag.NextStockId = nextId;
                ViewBag.NextCode    = $"{nextId}-{DateTime.Now.Year}";
                ChargerListes();

                var vm = new StockPageViewModel
                {
                    Stocks     = _context.TblStock.ToList(),
                    Mouvements = _context.TblStockMouvement.ToList()
                };

                return View("Index", vm);
            }


        // CREATE : ajout stock
        [HttpPost]
        public IActionResult Create(string ProductId, string CatId, int QtyDisponible, DateTime? DateMaj)
        {
            try
            {
                string nextId = GetNextStockId();

                var stock = new TblStock
                {
                    StockId       = nextId,
                    ProductId     = ProductId,
                    CatId         = CatId,
                    QtyDisponible = QtyDisponible,
                    DateMaj       = DateMaj ?? DateTime.Now,
                    CodeFormate   = $"{nextId}-{DateTime.Now.Year}"
                };

                _context.TblStock.Add(stock);
                _context.SaveChanges();

                TempData["Success"] = "Stock ajouté avec succès !";
            }
            catch (Exception ex)
            {
                TempData["Debug"] = "Erreur : " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        // IN : préparer formulaire entrée
            public IActionResult IN(string id)
                {
                    var stock = _context.TblStock.FirstOrDefault(s => s.StockId == id);
                    if (stock == null) return NotFound();

                    ChargerListes();
                    ViewBag.StockSelected = stock;
                    ViewBag.TypeMouvement = "IN";

                    string nextId = GetNextStockId();
                    ViewBag.NextStockId = nextId;
                    ViewBag.NextCode    = $"{nextId}-{DateTime.Now.Year}";

                    var vm = new StockPageViewModel
                    {
                        Stocks     = _context.TblStock.ToList(),
                        Mouvements = _context.TblStockMouvement.ToList()
                    };

                    return View("Index", vm);
                }

        // INConfirm : valider entrée
        [HttpPost]
        public IActionResult INConfirm(string stockId, string productId, int qty)
        {
            var stock = _context.TblStock.FirstOrDefault(s => s.StockId == stockId);
            if (stock == null) return NotFound();

            stock.QtyDisponible += qty;
            stock.DateMaj = DateTime.Now;

            var mouvement = new TblStockMouvement
            {
                MouvementId   = Guid.NewGuid().ToString().Substring(0, 10),
                ProductId     = productId,
                TypeMouvement = "IN",
                Qty           = qty,
                DateMouvement = DateTime.Now,
                RefVenteId    = null,
                Commentaire   = "Entrée de stock"
            };

            _context.TblStockMouvement.Add(mouvement);
            _context.SaveChanges();

            TempData["Success"] = "Entrée enregistrée !";
            return RedirectToAction("Index");
        }

        // OUT : préparer formulaire sortie
            public IActionResult OUT(string id)
                {
                    var stock = _context.TblStock.FirstOrDefault(s => s.StockId == id);
                    if (stock == null) return NotFound();

                    ChargerListes();
                    ViewBag.StockSelected = stock;
                    ViewBag.TypeMouvement = "OUT";

                    string nextId = GetNextStockId();
                    ViewBag.NextStockId = nextId;
                    ViewBag.NextCode    = $"{nextId}-{DateTime.Now.Year}";

                    var vm = new StockPageViewModel
                    {
                        Stocks     = _context.TblStock.ToList(),
                        Mouvements = _context.TblStockMouvement.ToList()
                    };

                    return View("Index", vm);
                }

        // OUTConfirm : valider sortie
        [HttpPost]
        public IActionResult OUTConfirm(string stockId, string productId, int qty)
        {
            var stock = _context.TblStock.FirstOrDefault(s => s.StockId == stockId);
            if (stock == null || stock.QtyDisponible < qty)
            {
                TempData["Debug"] = "Quantité insuffisante";
                return RedirectToAction("Index");
            }

            stock.QtyDisponible -= qty;
            stock.DateMaj = DateTime.Now;

            var mouvement = new TblStockMouvement
            {
                MouvementId   = Guid.NewGuid().ToString().Substring(0, 10),
                ProductId     = productId,
                TypeMouvement = "OUT",
                Qty           = qty,
                DateMouvement = DateTime.Now,
                RefVenteId    = null,
                Commentaire   = "Sortie de stock / Perte/Consommation"
            };

            _context.TblStockMouvement.Add(mouvement);
            _context.SaveChanges();

            TempData["Success"] = "Sortie enregistrée !";
            return RedirectToAction("Index");
        }

        // EDIT
        public IActionResult Edit(string id)
        {
            var stock = _context.TblStock.FirstOrDefault(s => s.StockId == id);
            if (stock == null) return NotFound();

            ChargerListes();
            ViewBag.EditStock = stock;

            var vm = new StockPageViewModel
            {
                Stocks     = _context.TblStock.ToList(),
                Mouvements = _context.TblStockMouvement.ToList()
            };

            return View("Index", vm);
        }

        [HttpPost]
        public IActionResult Edit(string StockId, string ProductId, string CatId, int QtyDisponible)
        {
            var stock = _context.TblStock.FirstOrDefault(s => s.StockId == StockId);
            if (stock != null)
            {
                stock.ProductId     = ProductId;
                stock.CatId         = CatId;
                stock.QtyDisponible = QtyDisponible;
                stock.DateMaj       = DateTime.Now;
                _context.SaveChanges();
                TempData["Success"] = "Stock modifié avec succès !";
            }
            return RedirectToAction("Index");
        }

        // DELETE
        public IActionResult Delete(string id)
        {
            var stock = _context.TblStock.FirstOrDefault(s => s.StockId == id);
            if (stock != null)
            {
                _context.TblStock.Remove(stock);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Génération ID auto
        private string GetNextStockId()
            {
                int nextNum = 1;

                if (_context.TblStock.Any())
                {
                    var maxId = _context.TblStock
                        .Select(s => s.StockId)
                        .ToList()
                        .Select(id =>
                        {
                            var parts = id.Split('-');
                            if (parts.Length > 1 && int.TryParse(parts[1], out int n))
                                return n;
                            return 0;
                        })
                        .DefaultIfEmpty(0)
                        .Max();

                    nextNum = maxId + 1;
                }

                return $"ST-{nextNum:D3}";
            }

        private void ChargerListes()
        {
            ViewBag.Products = _context.TblProduct.ToList();
        }
    }
}
