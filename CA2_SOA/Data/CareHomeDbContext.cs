using Microsoft.EntityFrameworkCore;
using CA2_SOA.Models;

namespace CA2_SOA.Data;

/// <summary>
/// Database context for the Elderly Care Home monitoring system
/// </summary>
public class CareHomeDbContext : DbContext
{
    public CareHomeDbContext(DbContextOptions<CareHomeDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Resident> Residents { get; set; }
    public DbSet<SensorData> SensorReadings { get; set; }
    public DbSet<Alert> Alerts { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure Room entity
        modelBuilder.Entity<Room>()
            .HasIndex(r => r.RoomNumber)
            .IsUnique();
        
        // Configure User entity
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        // Configure relationships
        modelBuilder.Entity<Resident>()
            .HasOne(r => r.Room)
            .WithMany(room => room.Residents)
            .HasForeignKey(r => r.RoomId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<SensorData>()
            .HasOne(s => s.Room)
            .WithMany(room => room.SensorReadings)
            .HasForeignKey(s => s.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Alert>()
            .HasOne(a => a.Room)
            .WithMany(room => room.Alerts)
            .HasForeignKey(a => a.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Seed initial data
        SeedData(modelBuilder);
    }
    
    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed default admin user (password: Admin123!)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FullName = "System Administrator",
                Email = "admin@carehome.com",
                Role = "Admin",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new User
            {
                Id = 2,
                Username = "caretaker1",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Care123!"),
                FullName = "John Smith",
                Email = "john.smith@carehome.com",
                Role = "Caretaker",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        );
        
        // Seed rooms
        modelBuilder.Entity<Room>().HasData(
            new Room
            {
                Id = 1,
                RoomNumber = "101",
                RoomName = "Rose Room",
                Floor = "1st Floor",
                Capacity = 1,
                IsOccupied = true,
                CreatedAt = DateTime.UtcNow
            },
            new Room
            {
                Id = 2,
                RoomNumber = "102",
                RoomName = "Lily Room",
                Floor = "1st Floor",
                Capacity = 2,
                IsOccupied = true,
                CreatedAt = DateTime.UtcNow
            },
            new Room
            {
                Id = 3,
                RoomNumber = "201",
                RoomName = "Orchid Room",
                Floor = "2nd Floor",
                Capacity = 1,
                IsOccupied = false,
                CreatedAt = DateTime.UtcNow
            }
        );
        
        // Seed residents
        modelBuilder.Entity<Resident>().HasData(
            new Resident
            {
                Id = 1,
                FirstName = "Mary",
                LastName = "Johnson",
                DateOfBirth = new DateTime(1940, 5, 15),
                MedicalConditions = "Diabetes, High Blood Pressure",
                EmergencyContact = "Sarah Johnson (Daughter)",
                EmergencyPhone = "+353-87-1234567",
                AdmissionDate = new DateTime(2023, 1, 10),
                RoomId = 1,
                IsActive = true
            },
            new Resident
            {
                Id = 2,
                FirstName = "James",
                LastName = "O'Brien",
                DateOfBirth = new DateTime(1938, 11, 22),
                MedicalConditions = "Arthritis, Dementia",
                EmergencyContact = "Michael O'Brien (Son)",
                EmergencyPhone = "+353-86-9876543",
                AdmissionDate = new DateTime(2023, 3, 5),
                RoomId = 2,
                IsActive = true
            }
        );
        
        // Seed sensor data
        modelBuilder.Entity<SensorData>().HasData(
            new SensorData
            {
                Id = 1,
                RoomId = 1,
                Temperature = 21.5m,
                Humidity = 45.0m,
                Timestamp = DateTime.UtcNow.AddHours(-2),
                SensorType = "DHT22"
            },
            new SensorData
            {
                Id = 2,
                RoomId = 2,
                Temperature = 22.0m,
                Humidity = 48.0m,
                Timestamp = DateTime.UtcNow.AddHours(-2),
                SensorType = "DHT22"
            },
            new SensorData
            {
                Id = 3,
                RoomId = 1,
                Temperature = 21.8m,
                Humidity = 46.0m,
                Timestamp = DateTime.UtcNow.AddHours(-1),
                SensorType = "DHT22"
            }
        );
        
        // Seed alerts
        modelBuilder.Entity<Alert>().HasData(
            new Alert
            {
                Id = 1,
                RoomId = 1,
                AlertType = "Temperature",
                Severity = "Low",
                Message = "Room temperature slightly below optimal range (21.5°C)",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                IsResolved = false
            }
        );
    }
}

