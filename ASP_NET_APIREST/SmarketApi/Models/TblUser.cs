using System.ComponentModel.DataAnnotations;

namespace SmarketApi.Models
{
public class TblUser
{
    [Key]
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public string? Role { get; set; }

    
}
}