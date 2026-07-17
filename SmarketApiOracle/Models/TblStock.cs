using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 


namespace SmarketApiOracle.Models
{
    public class TblStock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string StockId { get; set; } = "";      // nchar(10)
        public string? ProductId { get; set; }         // nvarchar(50)
        public int? QtyDisponible { get; set; }        // int
        public DateTime? DateMaj { get; set; }         // datetime
        public string? CatId { get; set; }             // nvarchar(50)
        public string? CodeFormate { get; set; } = "PENDING";      // nvarchar(50)
    }
}
