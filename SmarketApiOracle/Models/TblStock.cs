using System;
using System.ComponentModel.DataAnnotations;

namespace SmarketApiOracle.Models
{
    public class TblStock
    {
        [Key]
        public string StockId { get; set; } = "";      // nchar(10)
        public string? ProductId { get; set; }         // nvarchar(50)
        public int? QtyDisponible { get; set; }        // int
        public DateTime? DateMaj { get; set; }         // datetime
        public string? CatId { get; set; }             // nvarchar(50)
        public string? CodeFormate { get; set; }       // nvarchar(50)
    }
}
