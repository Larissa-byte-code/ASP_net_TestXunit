using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace SmarketApiOracle.Models
{
    public class TblCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //clé auto‑incrémentée gérée par la base.
        //cette clé est générée par la base
        public int CatId { get; set; }

        public  required string CatName { get; set; }
        public  required string CatDes { get; set; }
        public required string CatIdvC { get; set; } = "PENDING";

 

    }
}
