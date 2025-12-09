using System.ComponentModel.DataAnnotations;

namespace CA2_SOA.Models;

/// <summary>
/// Represents a room in the elderly care home
/// </summary>
public class Room
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string RoomNumber { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? RoomName { get; set; }
    
    [MaxLength(20)]
    public string Floor { get; set; } = "Ground";
    
    public int Capacity { get; set; } = 1;
    
    public bool IsOccupied { get; set; } = false;
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties - One-to-Many relationships
    public ICollection<Resident> Residents { get; set; } = new List<Resident>();
    public ICollection<SensorData> SensorReadings { get; set; } = new List<SensorData>();
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}

