using System.ComponentModel.DataAnnotations;
namespace MonProjetMVC.Models
{
    public class TblProduct
    {
        [Key]
        public int prdId { get; set; }
        public required string prdName { get; set; }
        public required string prdCat { get; set; }   // FK vers TblCategory
        public required string prdIdvC { get; set; }
    }
}
