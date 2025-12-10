using CA2_SOA.Data;
using CA2_SOA.Models;
using CA2_SOA.Repositories;
using CA2_SOA.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CA2_SOA.Tests.Repositories;

public class RoomRepositoryTests
{
    private readonly CareHomeDbContext _context;
    private readonly RoomRepository _repository;

    public RoomRepositoryTests()
    {
        _context = DbContextHelper.CreateInMemoryContext();
        _repository = new RoomRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllRooms()
    {
        // Arrange
        var rooms = new List<Room>
        {
            new Room { Id = 1, RoomNumber = "101", Floor = "1", Capacity = 2, CreatedAt = DateTime.UtcNow },
            new Room { Id = 2, RoomNumber = "102", Floor = "1", Capacity = 1, CreatedAt = DateTime.UtcNow }
        };
        await _context.Rooms.AddRangeAsync(rooms);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.RoomNumber == "101");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsRoom()
    {
        // Arrange
        var room = new Room
        {
            Id = 1,
            RoomNumber = "101",
            RoomName = "Rose Room",
            Floor = "1",
            Capacity = 2,
            CreatedAt = DateTime.UtcNow
        };
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.RoomNumber.Should().Be("101");
        result.RoomName.Should().Be("Rose Room");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_AddsRoomToDatabase()
    {
        // Arrange
        var room = new Room
        {
            RoomNumber = "103",
            RoomName = "Orchid Room",
            Floor = "2",
            Capacity = 1,
            Notes = "Test notes",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _repository.CreateAsync(room);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        var savedRoom = await _context.Rooms.FindAsync(result.Id);
        savedRoom.Should().NotBeNull();
        savedRoom!.RoomNumber.Should().Be("103");
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingRoom()
    {
        // Arrange
        var room = new Room
        {
            RoomNumber = "101",
            Floor = "1",
            Capacity = 2,
            CreatedAt = DateTime.UtcNow
        };
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        room.RoomName = "Updated Room";
        room.Capacity = 3;

        // Act
        var result = await _repository.UpdateAsync(room.Id, room);

        // Assert
        result.Should().NotBeNull();
        result!.RoomName.Should().Be("Updated Room");
        result.Capacity.Should().Be(3);
    }

    [Fact]
    public async Task DeleteAsync_RemovesRoomFromDatabase()
    {
        // Arrange
        var room = new Room
        {
            RoomNumber = "101",
            Floor = "1",
            Capacity = 2,
            CreatedAt = DateTime.UtcNow
        };
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(room.Id);

        // Assert
        result.Should().BeTrue();
        var deletedRoom = await _context.Rooms.FindAsync(room.Id);
        deletedRoom.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RoomNumberExistsAsync_WithExistingNumber_ReturnsTrue()
    {
        // Arrange
        var room = new Room
        {
            RoomNumber = "101",
            Floor = "1",
            Capacity = 2,
            CreatedAt = DateTime.UtcNow
        };
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.RoomNumberExistsAsync("101");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task RoomNumberExistsAsync_WithNonExistingNumber_ReturnsFalse()
    {
        // Act
        var result = await _repository.RoomNumberExistsAsync("999");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetOccupiedRoomsAsync_ReturnsOnlyOccupiedRooms()
    {
        // Arrange
        var rooms = new List<Room>
        {
            new Room { RoomNumber = "101", Floor = "1", Capacity = 2, IsOccupied = true, CreatedAt = DateTime.UtcNow },
            new Room { RoomNumber = "102", Floor = "1", Capacity = 1, IsOccupied = false, CreatedAt = DateTime.UtcNow },
            new Room { RoomNumber = "103", Floor = "2", Capacity = 2, IsOccupied = true, CreatedAt = DateTime.UtcNow }
        };
        await _context.Rooms.AddRangeAsync(rooms);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetOccupiedRoomsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(r => r.IsOccupied);
    }

    [Fact]
    public async Task GetAvailableRoomsAsync_ReturnsOnlyAvailableRooms()
    {
        // Arrange
        var rooms = new List<Room>
        {
            new Room { RoomNumber = "101", Floor = "1", Capacity = 2, IsOccupied = true, CreatedAt = DateTime.UtcNow },
            new Room { RoomNumber = "102", Floor = "1", Capacity = 1, IsOccupied = false, CreatedAt = DateTime.UtcNow },
            new Room { RoomNumber = "103", Floor = "2", Capacity = 2, IsOccupied = false, CreatedAt = DateTime.UtcNow }
        };
        await _context.Rooms.AddRangeAsync(rooms);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAvailableRoomsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(r => !r.IsOccupied);
    }

    [Fact]
    public async Task GetRoomWithDetailsAsync_ReturnsRoomWithRelatedData()
    {
        // Arrange
        var room = new Room
        {
            RoomNumber = "101",
            Floor = "1",
            Capacity = 2,
            CreatedAt = DateTime.UtcNow
        };
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        var resident = new Resident
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1940, 1, 1),
            AdmissionDate = DateTime.UtcNow,
            RoomId = room.Id,
            IsActive = true
        };
        await _context.Residents.AddAsync(resident);

        var sensorData = new SensorData
        {
            RoomId = room.Id,
            Temperature = 22.5M,
            Humidity = 45,
            Timestamp = DateTime.UtcNow,
            SensorType = "Environmental"
        };
        await _context.SensorReadings.AddAsync(sensorData);

        var alert = new Alert
        {
            RoomId = room.Id,
            AlertType = "Temperature",
            Severity = "Low",
            Message = "Test alert",
            CreatedAt = DateTime.UtcNow,
            IsResolved = false
        };
        await _context.Alerts.AddAsync(alert);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetRoomWithDetailsAsync(room.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Residents.Should().HaveCount(1);
        result.SensorReadings.Should().HaveCount(1);
        result.Alerts.Should().HaveCount(1);
    }
}

