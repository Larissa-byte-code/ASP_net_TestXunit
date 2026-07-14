using System.ComponentModel.DataAnnotations;

namespace MonProjetMVC.Models
{
    public class TblSelling
    {
        [Key]
        public int VenteId { get; set; }
        public DateTime DateVente { get; set; }
        public required string SellerName { get; set; }
        public required string ClientName { get; set; }
        public required string Numfacture { get; set; }
        public required string ModeDePaiement { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
