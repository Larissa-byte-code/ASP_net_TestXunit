using System.ComponentModel.DataAnnotations;

namespace SmarketApiOracle.Models
{
    public class TblCategory
    {
        [Key]   // Indique la clé primaire
        public int CatId { get; set; }

        public required string CatName { get; set; }
        public required string CatDes { get; set; }
        public required string CatIdvC { get; set; }
    }
}
