using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace SmarketApiOracle.Models
{
    public class TblDetailSelling
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetailId { get; set; }
        public int VenteId { get; set; }    // FK vers TblSelling
        public string ProductId { get; set; }  // FK vers TblProduct
        public string ProductName { get; set; } = string.Empty;
        //public string Category { get; set; } = string.Empty;
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }
    }
}
