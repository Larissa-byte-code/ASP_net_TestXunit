using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace SmarketApiOracle.Models
{
    public class TblSeller
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SellerId { get; set; }
        public required string SellerName { get; set; }
        public int SellerAge { get; set; }
        public required string SellerPhone { get; set; }
        public required string SellerPass { get; set; }
        public required string SellerIdvC { get; set; }= "PENDING";
        public required string Role { get; set; }
    }
}
