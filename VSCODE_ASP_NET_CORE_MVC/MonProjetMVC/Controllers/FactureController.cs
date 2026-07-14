using Microsoft.AspNetCore.Mvc;
using MonProjetMVC.Data;
using MonProjetMVC.Models;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Text;

namespace MonProjetMVC.Controllers
{
    public class FactureController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConverter _converter;

        public FactureController(ApplicationDbContext context, IConverter converter)
        {
            _context  = context;
            _converter = converter;
        }

        // Affiche la facture en HTML (apercu)
        public IActionResult Index(int id)
        {
            var vente = _context.TblSelling.FirstOrDefault(v => v.VenteId == id);
            if (vente == null) return NotFound();

            var details = _context.TblDetailSelling
                .Where(d => d.VenteId == id)
                .ToList();

            ViewBag.Details = details;
            return View(vente);
        }

        // Telecharge la facture en PDF
        public IActionResult Pdf(int id)
        {
            var vente = _context.TblSelling.FirstOrDefault(v => v.VenteId == id);
            if (vente == null) return NotFound();

            var details = _context.TblDetailSelling
                .Where(d => d.VenteId == id)
                .ToList();

            // Generer le HTML de la facture
            string html = GenererHtmlFacture(vente, details);

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
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            byte[] pdf = _converter.Convert(doc);
            return File(pdf, "application/pdf", $"Facture-{vente.Numfacture}.pdf");
        }

        private string GenererHtmlFacture(TblSelling vente, List<TblDetailSelling> details)
        {
            var sb = new StringBuilder();

            sb.Append(@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <style>
                    body        { font-family: Arial, sans-serif; font-size: 13px; color: #333; }
                    .header     { display: flex; justify-content: space-between; margin-bottom: 30px; }
                    .title      { font-size: 36px; font-weight: bold; letter-spacing: 4px; color: #222; }
                    .info-block { margin-bottom: 20px; }
                    .info-block h4 { font-size: 13px; color: #888; margin: 0 0 4px 0; text-transform: uppercase; }
                    .info-block p  { margin: 0; font-size: 14px; }
                    .row-info   { display: flex; gap: 60px; margin-bottom: 30px; }
                    table       { width: 100%; border-collapse: collapse; margin-bottom: 20px; }
                    thead th    { background: #e74c3c; color: white; padding: 10px; text-align: left; font-size: 13px; }
                    tbody td    { padding: 9px 10px; border-bottom: 1px solid #eee; }
                    tbody tr:nth-child(even) { background: #fafafa; }
                    .total-box  { text-align: right; margin-top: 10px; }
                    .total-box .total { font-size: 18px; font-weight: bold; color: #e74c3c; }
                    .footer     { margin-top: 40px; text-align: center; color: #888; font-style: italic; font-size: 12px; }
                    .paiement   { margin-top: 10px; font-size: 13px; }
                    hr          { border: none; border-top: 1px solid #eee; margin: 20px 0; }
                </style>
            </head>
            <body>
            ");

            // En-tete
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
            ");

            // Tableau des details
            sb.Append(@"
            <table>
                <thead>
                    <tr>
                        <th>Designation</th>
                        <th>Quantite</th>
                        <th>Prix unitaire</th>
                        <th>Montant HT</th>
                    </tr>
                </thead>
                <tbody>
            ");

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

            sb.Append("</tbody></table>");

            // Total et paiement
            sb.Append($@"
            <hr/>
            <div class='total-box'>
                <p>Montant total : <span class='total'>{vente.TotalAmount:N2} Ar</span></p>
            </div>
            <div class='paiement'>
                <strong>Mode de paiement :</strong> {vente.ModeDePaiement}
            </div>

            <div class='footer'>
                <p style='font-size:40px; color: SlateBlue; font-family: cursive;'>merci</p>
                <p>IBAN: MG12 1234 5678 &nbsp;|&nbsp; SWIFT/BIC: ABCXXXXXX</p>
            </div>

            </body>
            </html>
            ");

            return sb.ToString();
        }
    }
}