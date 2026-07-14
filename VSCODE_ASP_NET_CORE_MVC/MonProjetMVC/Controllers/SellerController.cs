using Microsoft.AspNetCore.Mvc;
using MonProjetMVC.Data;
using MonProjetMVC.Models;

namespace MonProjetMVC.Controllers
{
    public class SellerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SellerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTE
        public IActionResult Index()
        {
            var sellers = _context.TblSeller.ToList();
            return View(sellers);
        }

        // CREATE (Add)
        [HttpPost]
        public IActionResult Create(TblSeller seller)
        {
            if (ModelState.IsValid)
            {
                int nextId = (_context.TblSeller.Max(s => (int?)s.SellerId) ?? 0) + 1;
                string year = DateTime.Now.Year.ToString();
                seller.SellerId = nextId;
                seller.SellerIdvC = $"Sel-{nextId:D3}-{year}";

                // Préfixe automatique pour Madagascar
                if (!string.IsNullOrEmpty(seller.SellerPhone) && !seller.SellerPhone.StartsWith("+261"))
                {
                    seller.SellerPhone = $"+261{seller.SellerPhone}";
                }

                _context.TblSeller.Add(seller);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // NEW
        public IActionResult New()
        {
            int nextId = (_context.TblSeller.Max(s => (int?)s.SellerId) ?? 0) + 1;
            string year = DateTime.Now.Year.ToString();
            string code = $"Sel-{nextId:D3}-{year}";

            ViewBag.NextId = nextId;
            ViewBag.NextCode = code;

            var sellers = _context.TblSeller.ToList();
            return View("Index", sellers);
        }

        // EDIT (GET)
        public IActionResult Edit(int id)
        {
            var seller = _context.TblSeller.Find(id);
            if (seller == null) return NotFound();

            ViewBag.EditSeller = seller;

            var sellers = _context.TblSeller.ToList();
            return View("Index", sellers);
        }

        // EDIT (POST)
        [HttpPost]
        public IActionResult Edit(TblSeller seller)
        {
            if (ModelState.IsValid)
            {
                var existing = _context.TblSeller.Find(seller.SellerId);
                if (existing != null)
                {
                    existing.SellerName = seller.SellerName;
                    existing.SellerAge = seller.SellerAge;
                    existing.SellerPhone = seller.SellerPhone.StartsWith("+261") ? seller.SellerPhone : $"+261{seller.SellerPhone}";
                    existing.SellerPass = seller.SellerPass;
                    existing.Role = seller.Role;
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            var seller = _context.TblSeller.Find(id);
            if (seller != null)
            {
                _context.TblSeller.Remove(seller);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
