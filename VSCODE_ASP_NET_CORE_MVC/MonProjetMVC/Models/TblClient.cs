using System.ComponentModel.DataAnnotations;
namespace MonProjetMVC.Models
{
    public class TblClient
    {
        [Key]
        public int ClientId { get; set; }
        public required string ClientCode { get; set; }
        public required string ClientName { get; set; }
        public required string ClientPhone { get; set; }
        public required string ClientEmail { get; set; }
        public required string ClientAddress { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsActive { get; set; }
    }
}
