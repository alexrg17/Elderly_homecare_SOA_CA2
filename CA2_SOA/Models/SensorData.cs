using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CA2_SOA.Models;

/// <summary>
/// Represents environmental sensor data readings from a room
/// </summary>
public class SensorData
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int RoomId { get; set; }
    
    [Required]
    public decimal Temperature { get; set; } // in Celsius
    
    [Required]
    public decimal Humidity { get; set; } // percentage
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [MaxLength(50)]
    public string SensorType { get; set; } = "DHT22";
    
    [MaxLength(200)]
    public string? Notes { get; set; }
    
    // Navigation property - Many-to-One relationship with Room
    [ForeignKey("RoomId")]
    public Room Room { get; set; } = null!;
}

