using System.Collections.Generic;
using MonProjetMVC.Models;
namespace MonProjetMVC.Models   
{
    public class VenteIndexViewModel
    {
        public TblSelling Selling { get; set; } = new TblSelling
        {
            SellerName = string.Empty,
            ClientName = string.Empty,
            Numfacture = string.Empty,
            ModeDePaiement = string.Empty,
            DateVente = DateTime.Now,
            TotalAmount = 0
        };

        public List<TblDetailSelling> Details { get; set; } = new List<TblDetailSelling>();
    }
}
