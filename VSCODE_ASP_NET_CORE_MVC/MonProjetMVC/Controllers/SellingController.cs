using Microsoft.AspNetCore.Mvc;
using MonProjetMVC.Data;
using MonProjetMVC.Models;
using DinkToPdf;                    // bibliotheque de generation PDF
using DinkToPdf.Contracts;         // interface IConverter
using System.Text;                 // pour StringBuilder

namespace MonProjetMVC.Controllers
{
    public class SellingController : Controller
    {
        private readonly ApplicationDbContext _context;
        // IConverter = interface DinkToPdf injectee via Program.cs
        // permet de convertir du HTML en PDF
        private readonly IConverter _converter;

        // Le constructeur recoit les deux services injectes automatiquement
        // _context  = connexion base de donnees
        // _converter = convertisseur HTML→PDF
        public SellingController(ApplicationDbContext context, IConverter converter)
        {
            _context   = context;
            _converter = converter;
        }

        // INDEX - charge la page principale avec un formulaire vide
        public IActionResult Index()
        {
            ChargerListes(); // charge produits, vendeurs, clients dans ViewBag

            // Calcule le prochain ID de vente
            int nextId = _context.TblSelling.Any()
                ? _context.TblSelling.Max(s => s.VenteId) + 1
                : 1;
            string year = DateTime.Now.Year.ToString();

            // Envoie l'ID et le numero de facture preview a la vue
            ViewBag.NextId      = nextId;
            ViewBag.NextInvoice = $"Facture-{nextId:D3}-{year}";

            // Cree un modele vide pour le formulaire
            var model = new VenteIndexViewModel
            {
                Selling = new TblSelling
                {
                    DateVente      = DateTime.Now,
                    SellerName     = string.Empty,
                    ClientName     = string.Empty,
                    Numfacture     = string.Empty,
                    ModeDePaiement = string.Empty,
                    TotalAmount    = 0
                },
                // Une ligne vide dans le tableau de details
                Details = new List<TblDetailSelling> { new TblDetailSelling() }
            };

            return View(model); // retourne Views/Selling/Index.cshtml
        }

        // NEW - remet le formulaire a zero
        public IActionResult New()
        {
            // Redirige vers Index qui regenere un nouvel ID
            return RedirectToAction("Index");
        }

        // OUT - enregistre la vente en base (POST car soumission formulaire)
        [HttpPost]
        public IActionResult Out(VenteIndexViewModel model)
        {
            ChargerListes();

            // Retire ces champs de la validation car ils sont generes cote serveur
            ModelState.Remove("Selling.VenteId");
            ModelState.Remove("Selling.Numfacture");
            ModelState.Remove("Selling.TotalAmount");

            // Si le formulaire contient des erreurs de validation
            if (!ModelState.IsValid)
            {
                // Concatene toutes les erreurs dans TempData pour les afficher
                TempData["Debug"] = string.Join(" | ",
                    ModelState
                        .Where(x => x.Value!.Errors.Count > 0)
                        .Select(x => $"{x.Key}: {x.Value!.Errors.First().ErrorMessage}")
                );

                int nIdErr = _context.TblSelling.Any()
                    ? _context.TblSelling.Max(s => s.VenteId) + 1
                    : 1;
                ViewBag.NextId      = nIdErr;
                ViewBag.NextInvoice = $"Facture-{nIdErr:D3}-{DateTime.Now.Year}";
                return View("Index", model); // reaffiche le formulaire avec les erreurs
            }

            // BeginTransaction = regroupe plusieurs SaveChanges en une seule transaction
            // Si une etape echoue, Rollback() annule tout
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // VenteId = 0 car SQL Server le genere automatiquement (IDENTITY)
                model.Selling.VenteId   = 0;

                // Si la date n'a pas ete saisie, utilise la date du jour
                model.Selling.DateVente = model.Selling.DateVente == default
                    ? DateTime.Now
                    : model.Selling.DateVente;

                // Calcule le total a partir des lignes de detail (plus fiable que le champ HTML)
                model.Selling.TotalAmount = model.Details
                    .Where(d => d.ProductId != 0 && d.Qty > 0)
                    .Sum(d => d.Qty * d.Price);

                // Numfacture provisoire car on ne connait pas encore le vrai VenteId
                model.Selling.Numfacture = "TEMP";

                _context.TblSelling.Add(model.Selling);
                _context.SaveChanges();
                // Apres SaveChanges, EF Core a recupere le VenteId genere par SQL Server
                // et l'a mis dans model.Selling.VenteId automatiquement

                int venteId = model.Selling.VenteId; // le vrai ID genere
                string year = DateTime.Now.Year.ToString();

                // Maintenant on peut construire le vrai numero de facture
                model.Selling.Numfacture = $"Facture-{venteId:D3}-{year}";
                _context.SaveChanges(); // UPDATE TblSelling SET Numfacture = ... WHERE VenteId = ...

                // Parcourir chaque ligne de detail du tableau produits
                foreach (var detail in model.Details)
                {
                    // Ignorer les lignes vides
                    if (detail.ProductId == 0 || detail.Qty <= 0) continue;

                    detail.VenteId   = venteId;          // lier le detail a la vente
                    detail.LineTotal = detail.Qty * detail.Price; // calculer le sous-total
                    _context.TblDetailSelling.Add(detail);

                    // ProductId dans TblStock est en string, on convertit
                    string productIdStr = detail.ProductId.ToString();

                    // Chercher le stock correspondant au produit
                    var stock = _context.TblStock
                        .FirstOrDefault(s => s.ProductId == productIdStr);

                    // Deduire la quantite vendue du stock si suffisant
                    if (stock != null && stock.QtyDisponible >= detail.Qty)
                    {
                        stock.QtyDisponible -= detail.Qty;
                        stock.DateMaj = DateTime.Now;
                    }

                    // Creer un mouvement de stock de type OUT
                    var mouvement = new TblStockMouvement
                    {
                        // Guid.NewGuid() = identifiant unique global (ex: "a1b2c3d4e5")
                        // .Substring(0,10) = limite a 10 caracteres pour le champ nchar(10)
                        MouvementId   = Guid.NewGuid().ToString().Substring(0, 10),
                        ProductId     = productIdStr,
                        TypeMouvement = "OUT",
                        Qty           = detail.Qty,
                        DateMouvement = DateTime.Now,
                        RefVenteId    = venteId.ToString(), // reference vers la vente
                        Commentaire   = "Sortie de stock / Vente"
                    };
                    _context.TblStockMouvement.Add(mouvement);
                }

                _context.SaveChanges(); // INSERT INTO TblDetailSelling + TblStockMouvement
                transaction.Commit();   // valide toute la transaction en base

                // TempData survit a un redirect (contrairement a ViewBag)
                TempData["Success"]  = $"Vente enregistree ! Facture : {model.Selling.Numfacture}";
                TempData["VenteId"]  = venteId; // pour afficher le bouton facture apres redirect
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                transaction.Rollback(); // annule tout si erreur
                TempData["Debug"] = "Erreur : " + ex.Message +
                    (ex.InnerException != null ? " | Inner: " + ex.InnerException.Message : "");

                int nId = _context.TblSelling.Any()
                    ? _context.TblSelling.Max(s => s.VenteId) + 1
                    : 1;
                ViewBag.NextId      = nId;
                ViewBag.NextInvoice = $"Facture-{nId:D3}-{DateTime.Now.Year}";
                return View("Index", model);
            }
        }

        // INVOICE - affiche la facture en HTML dans un nouvel onglet
        public IActionResult Invoice(int id)
        {
            // Find(id) = SELECT * FROM TblSelling WHERE VenteId = id
            var vente = _context.TblSelling.Find(id);
            if (vente == null) return NotFound(); // retourne HTTP 404 si introuvable

            // Charge les lignes de detail de cette vente
            var details = _context.TblDetailSelling
                .Where(d => d.VenteId == id)
                .ToList();

            ViewBag.Details = details; // envoie les details a la vue via ViewBag
            return View(vente);        // retourne Views/Selling/Invoice.cshtml avec TblSelling
        }

        // PDF - genere et telecharge la facture en PDF
        public IActionResult Pdf(int id)
        {
            // Meme logique que Invoice mais genere un PDF au lieu d'une vue HTML
            var vente = _context.TblSelling.Find(id);
            if (vente == null) return NotFound();

            var details = _context.TblDetailSelling
                .Where(d => d.VenteId == id)
                .ToList();

            // Genere le HTML de la facture sous forme de string
            string html = GenererHtmlFacture(vente, details);

            // Configure le document PDF
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode   = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize   = PaperKind.A4,
                    Margins     = new MarginSettings { Top = 20, Bottom = 20, Left = 20, Right = 20 }
                },
                Objects = {
                    new ObjectSettings {
                        HtmlContent = html,                          // le HTML a convertir
                        WebSettings = { DefaultEncoding = "utf-8" } // encodage pour les accents
                    }
                }
            };

            // Convertit le HTML en tableau d'octets (le fichier PDF)
            byte[] pdf = _converter.Convert(doc);

            // Retourne le fichier PDF au navigateur avec le nom du fichier
            return File(pdf, "application/pdf", $"Facture-{vente.Numfacture}.pdf");
        }

        // Genere le HTML de la facture sous forme de string
        // Utilise StringBuilder pour concatener les morceaux de HTML efficacement
        private string GenererHtmlFacture(TblSelling vente, List<TblDetailSelling> details)
        {
            var sb = new StringBuilder();

            // HTML complet avec styles CSS integres (pas de fichier externe)
            sb.Append(@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <style>
                    body     { font-family: Arial, sans-serif; font-size: 13px; color: #333; margin: 30px; }
                    .header  { display: flex; justify-content: space-between; margin-bottom: 30px; }
                    .title   { font-size: 36px; font-weight: bold; letter-spacing: 4px; }
                    .info-block { margin-bottom: 20px; }
                    .info-block h4 { font-size: 12px; color: #888; margin: 0 0 4px 0; text-transform: uppercase; }
                    .info-block p  { margin: 0; font-size: 14px; }
                    .row-info { display: flex; gap: 60px; margin-bottom: 30px; }
                    table    { width: 100%; border-collapse: collapse; margin-bottom: 20px; }
                    thead th { background: #e74c3c; color: white; padding: 10px; text-align: left; border: 1px solid #ccc; }
                    tbody td { padding: 9px 10px; border: 1px solid #ccc; }
                    .total-box { text-align: right; }
                    .total   { font-size: 18px; font-weight: bold; color: #e74c3c; }
                    .footer  { margin-top: 40px; text-align: center; color: #888; font-style: italic; }
                    .paiement { margin-top: 10px; }
                </style>
            </head>
            <body>
            ");

            // Injection des donnees de la vente dans le HTML via interpolation $""
            sb.Append($@"
            <div class='header'>
                <div class='title'>FACTURE</div>
                <div>
                    <div class='info-block'>
                        <h4>Facture N°</h4>
                        <p><strong>{vente.Numfacture}</strong></p>
                    </div>
                    <div class='info-block'>
                        <h4>Date</h4>
                        <p>{vente.DateVente:dd/MM/yyyy}</p>
                    </div>
                </div>
            </div>
            <div class='row-info'>
                <div class='info-block'>
                    <h4>Facture de</h4>
                    <p>{vente.SellerName}</p>
                </div>
                <div class='info-block'>
                    <h4>Facture à</h4>
                    <p>{vente.ClientName}</p>
                </div>
            </div>
            <hr/>
            <table>
                <thead>
                    <tr>
                        <th>Désignation</th>
                        <th>Quantité</th>
                        <th>Prix unitaire</th>
                        <th>Montant HT</th>
                    </tr>
                </thead>
                <tbody>
            ");

            // Boucle sur chaque ligne de detail pour construire les lignes du tableau
            foreach (var d in details)
            {
                sb.Append($@"
                <tr>
                    <td>{d.ProductName}</td>
                    <td>{d.Qty}</td>
                    <td>{d.Price:N2}</td>
                    <td>{d.LineTotal:N2}</td>
                </tr>
                ");
            }

            // Fermeture du tableau et affichage du total + pied de page
            sb.Append($@"
                </tbody>
            </table>
            <hr/>
            <div class='total-box'>
                <p>Montant total : <span class='total'>{vente.TotalAmount:N2} Ar</span></p>
            </div>
            <div class='paiement'>
                <strong>Mode de paiement :</strong> {vente.ModeDePaiement}
            </div>
            <div class='footer'>
                <p style='font-size:40px; color:SlateBlue; font-family:cursive;'>merci</p>
                <p>IBAN: MG12 1234 5678 | SWIFT/BIC: ABCXXXXXX</p>
            </div>
            </body>
            </html>
            ");

            return sb.ToString(); // retourne le HTML complet sous forme de string
        }

        // Charge les listes deroulantes dans ViewBag pour le formulaire
        private void ChargerListes()
        {
            ViewBag.Products       = _context.TblProduct.ToList();
            ViewBag.Sellers        = _context.TblSeller.ToList();
            ViewBag.Customers      = _context.TblClient.ToList();
            ViewBag.PaymentMethods = new List<string> 
            { 
                "Especes", "Carte bancaire", "Cheque", "Virement", "Mobile Money" 
            };
        }
    }
}