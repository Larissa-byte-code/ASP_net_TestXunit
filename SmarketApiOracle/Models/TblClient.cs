using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace SmarketApiOracle.Models
{
    public class TblClient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ClientId { get; set; }
        public required string ClientCode { get; set; }= "PENDING";
        public required string ClientName { get; set; }
        public required string ClientPhone { get; set; }
        public required string ClientEmail { get; set; }
        public required string ClientAddress { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
        
    }
}
