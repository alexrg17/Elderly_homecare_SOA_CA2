using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA2_SOA.Models;

/// <summary>
/// Represents an elderly resident in the care home
/// </summary>
public class Resident
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
    
    public DateTime DateOfBirth { get; set; }
    
    [MaxLength(500)]
    public string? MedicalConditions { get; set; }
    
    [MaxLength(200)]
    public string? EmergencyContact { get; set; }
    
    [MaxLength(20)]
    public string? EmergencyPhone { get; set; }
    
    public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
    
    // Foreign Key
    public int? RoomId { get; set; }
    
    // Navigation property - Many-to-One relationship with Room
    [ForeignKey("RoomId")]
    public Room? Room { get; set; }
}

