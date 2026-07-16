using System.ComponentModel.DataAnnotations;
namespace SmarketApiOracle.Models
{
    public class TblDetailSelling
    {
        [Key]
        public int DetailId { get; set; }
        public int VenteId { get; set; }    // FK vers TblSelling
        public int ProductId { get; set; }  // FK vers TblProduct
        public string ProductName { get; set; } = string.Empty;
        //public string Category { get; set; } = string.Empty;
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }
    }
}
