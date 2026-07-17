using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 


namespace SmarketApiOracle.Models
{
    public class TblSelling
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VenteId { get; set; }
        public DateTime DateVente { get; set; }
        public required string SellerName { get; set; }
        public required string ClientName { get; set; }
        public required string Numfacture { get; set; }= "PENDING";
        public required string ModeDePaiement { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
