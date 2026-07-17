using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace SmarketApiOracle.Models
{
    public class TblProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int prdId { get; set; }
        public required string prdName { get; set; }
        public required string prdCat { get; set; }   
        public required string prdIdvC { get; set; }= "PENDING";
    }
}
