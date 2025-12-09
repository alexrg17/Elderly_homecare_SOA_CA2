using System.ComponentModel.DataAnnotations;
}
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string Role { get; set; } = "Caretaker"; // Admin, Caretaker, Viewer
    [MaxLength(50)]
    [Required]
    
    public string Email { get; set; } = string.Empty;
    [MaxLength(150)]
    [EmailAddress]
    [Required]
    
    public string FullName { get; set; } = string.Empty;
    [MaxLength(100)]
    [Required]
    
    public string PasswordHash { get; set; } = string.Empty;
    [MaxLength(255)]
    [Required]
    
    public string Username { get; set; } = string.Empty;
    [MaxLength(100)]
    [Required]
    
    public int Id { get; set; }
    [Key]
{
public class User
/// </summary>
/// Represents a user in the system (caretaker, admin, etc.)
/// <summary>

namespace CA2_SOA.Models;


