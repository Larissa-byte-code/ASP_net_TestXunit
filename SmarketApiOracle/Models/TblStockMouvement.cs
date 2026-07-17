using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 


namespace SmarketApiOracle.Models
{
    public class TblStockMouvement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string MouvementId { get; set; } = "";  // nchar(10)
        public string? ProductId { get; set; }         // nvarchar(50)
        public string? TypeMouvement { get; set; }     // nchar(10)
        public int? Qty { get; set; }                  // int
        public DateTime? DateMouvement { get; set; }   // datetime
        public string? RefVenteId { get; set; }        // nvarchar(50)
        public string? Commentaire { get; set; }       // nvarchar(50)
    }
}
