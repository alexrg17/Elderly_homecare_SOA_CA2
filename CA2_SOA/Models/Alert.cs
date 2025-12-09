using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA2_SOA.Models;

/// <summary>
/// Represents an alert generated when sensor readings are outside acceptable ranges
/// </summary>
public class Alert
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int RoomId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string AlertType { get; set; } = string.Empty; // Temperature, Humidity, etc.
    
    [Required]
    [MaxLength(20)]
    public string Severity { get; set; } = "Medium"; // Low, Medium, High, Critical
    
    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsResolved { get; set; } = false;
    
    public DateTime? ResolvedAt { get; set; }
    
    public int? ResolvedByUserId { get; set; }
    
    [MaxLength(500)]
    public string? ResolutionNotes { get; set; }
    
    // Navigation property - Many-to-One relationship with Room
    [ForeignKey("RoomId")]
    public Room Room { get; set; } = null!;
}

