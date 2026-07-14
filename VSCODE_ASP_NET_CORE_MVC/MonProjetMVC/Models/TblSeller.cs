using System.ComponentModel.DataAnnotations;
namespace MonProjetMVC.Models
{
    public class TblSeller
    {
        [Key]
        public int SellerId { get; set; }
        public required string SellerName { get; set; }
        public int SellerAge { get; set; }
        public required string SellerPhone { get; set; }
        public required string SellerPass { get; set; }
        public required string SellerIdvC { get; set; }
        public required string Role { get; set; }
    }
}
