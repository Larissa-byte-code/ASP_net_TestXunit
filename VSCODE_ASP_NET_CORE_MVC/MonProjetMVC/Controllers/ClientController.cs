using Microsoft.AspNetCore.Mvc;
using MonProjetMVC.Data;
using MonProjetMVC.Models;

namespace MonProjetMVC.Controllers
{
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LISTE
        public IActionResult Index()
        {
            var clients = _context.TblClient.ToList();
            int nextId = _context.TblClient.Any()
                ? _context.TblClient.Max(c => c.ClientId) + 1
                : 1;
            ViewBag.NextId   = nextId;
            ViewBag.NextCode = $"CL-{nextId:D3}-{DateTime.Now.Year}";
            return View(clients);
        }

        // NEW
        public IActionResult New()
        {
            int nextId = _context.TblClient.Any()
                ? _context.TblClient.Max(c => c.ClientId) + 1
                : 1;
            ViewBag.NextId   = nextId;
            ViewBag.NextCode = $"CL-{nextId:D3}-{DateTime.Now.Year}";
            var clients = _context.TblClient.ToList();
            return View("Index", clients);
        }

        // CREATE POST
        [HttpPost]
        public IActionResult Create(TblClient client)
        {
            ModelState.Remove("ClientCode");
            ModelState.Remove("ClientId");
            ModelState.Remove("DateCreated");
            ModelState.Remove("IsActive");

            if (ModelState.IsValid)
            {
                int nextId = _context.TblClient.Any()
                    ? _context.TblClient.Max(c => c.ClientId) + 1
                    : 1;

                client.ClientId    = nextId;
                client.ClientCode  = $"CL-{nextId:D3}-{DateTime.Now.Year}";
                client.DateCreated = DateTime.Now;
                client.IsActive    = Request.Form.ContainsKey("IsActive");

                _context.TblClient.Add(client);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // Si erreur afficher debug
            TempData["Debug"] = string.Join(" | ",
                ModelState
                    .Where(x => x.Value!.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {x.Value!.Errors.First().ErrorMessage}")
            );

            int nId = _context.TblClient.Any()
                ? _context.TblClient.Max(c => c.ClientId) + 1
                : 1;
            ViewBag.NextId   = nId;
            ViewBag.NextCode = $"CL-{nId:D3}-{DateTime.Now.Year}";
            return View("Index", _context.TblClient.ToList());
        }

        // EDIT GET
        public IActionResult Edit(int id)
        {
            var client = _context.TblClient.Find(id);
            if (client == null) return NotFound();
            ViewBag.EditClient = client;
            return View("Index", _context.TblClient.ToList());
        }

        // EDIT POST
        [HttpPost]
        [ActionName("Edit")]
        public IActionResult EditPost(TblClient client)
        {
            ModelState.Remove("ClientCode");
            ModelState.Remove("ClientId");
            ModelState.Remove("DateCreated");
            ModelState.Remove("IsActive");

            if (ModelState.IsValid)
            {
                var existing = _context.TblClient
                    .FirstOrDefault(c => c.ClientId == client.ClientId);

                if (existing != null)
                {
                    existing.ClientName    = client.ClientName;
                    existing.ClientPhone   = client.ClientPhone;
                    existing.ClientEmail   = client.ClientEmail;
                    existing.ClientAddress = client.ClientAddress;
                    existing.IsActive      = Request.Form.ContainsKey("IsActive");
                    _context.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            TempData["Debug"] = string.Join(" | ",
                ModelState
                    .Where(x => x.Value!.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {x.Value!.Errors.First().ErrorMessage}")
            );

            ViewBag.EditClient = client;
            return View("Index", _context.TblClient.ToList());
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            var client = _context.TblClient.Find(id);
            if (client != null)
            {
                _context.TblClient.Remove(client);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}