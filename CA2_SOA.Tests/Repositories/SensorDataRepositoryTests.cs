using Xunit;
using FluentAssertions;
using CA2_SOA.Models;
using CA2_SOA.Repositories;
using CA2_SOA.Tests.Helpers;

namespace CA2_SOA.Tests.Repositories;

/// <summary>
/// Unit tests for SensorDataRepository
/// Tests sensor data retrieval, filtering, and CRUD operations
/// </summary>
public class SensorDataRepositoryTests : IDisposable
{
    private readonly SensorDataRepository _repository;
    private readonly Data.CareHomeDbContext _context;

    public SensorDataRepositoryTests()
    {
        _context = DbContextHelper.CreateInMemoryContext();
        _repository = new SensorDataRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateSensorData()
    {
        // Arrange
        var room = new Room { RoomNumber = "201", Floor = "2", Capacity = 1 };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var sensorData = new SensorData
        {
            RoomId = room.Id,
            Temperature = 22.5M,
            Humidity = 45.0M,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var result = await _repository.CreateAsync(sensorData);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Temperature.Should().Be(22.5M);
        result.Humidity.Should().Be(45.0M);
    }

    [Fact]
    public async Task GetByRoomIdAsync_ShouldReturnSensorDataForRoom()
    {
        // Arrange
        var room1 = new Room { RoomNumber = "202", Floor = "2", Capacity = 1 };
        var room2 = new Room { RoomNumber = "203", Floor = "2", Capacity = 1 };
        _context.Rooms.AddRange(room1, room2);
        await _context.SaveChangesAsync();

        await _repository.CreateAsync(new SensorData { RoomId = room1.Id, Temperature = 21.0M, Humidity = 40.0M, Timestamp = DateTime.UtcNow });
        await _repository.CreateAsync(new SensorData { RoomId = room1.Id, Temperature = 22.0M, Humidity = 42.0M, Timestamp = DateTime.UtcNow.AddMinutes(1) });
        await _repository.CreateAsync(new SensorData { RoomId = room2.Id, Temperature = 23.0M, Humidity = 44.0M, Timestamp = DateTime.UtcNow });

        // Act
        var result = await _repository.GetByRoomIdAsync(room1.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(s => s.RoomId.Should().Be(room1.Id));
    }

    [Fact]
    public async Task GetLatestByRoomIdAsync_ShouldReturnMostRecentReading()
    {
        // Arrange
        var room = new Room { RoomNumber = "204", Floor = "2", Capacity = 1 };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        await _repository.CreateAsync(new SensorData { RoomId = room.Id, Temperature = 20.0M, Humidity = 38.0M, Timestamp = DateTime.UtcNow.AddHours(-2) });
        await _repository.CreateAsync(new SensorData { RoomId = room.Id, Temperature = 21.0M, Humidity = 40.0M, Timestamp = DateTime.UtcNow.AddHours(-1) });
        await _repository.CreateAsync(new SensorData { RoomId = room.Id, Temperature = 22.0M, Humidity = 42.0M, Timestamp = DateTime.UtcNow });

        // Act
        var result = await _repository.GetLatestByRoomIdAsync(room.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Temperature.Should().Be(22.0M);
        result.Humidity.Should().Be(42.0M);
    }

    [Fact]
    public async Task GetRecentReadingsAsync_ShouldReturnLimitedResults()
    {
        // Arrange
        var room = new Room { RoomNumber = "205", Floor = "2", Capacity = 1 };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        // Add 10 readings
        for (int i = 0; i < 10; i++)
        {
            await _repository.CreateAsync(new SensorData 
            { 
                RoomId = room.Id, 
                Temperature = 20.0M + i, 
                Humidity = 40.0M, 
                Timestamp = DateTime.UtcNow.AddMinutes(i) 
            });
        }

        // Act
        var result = await _repository.GetRecentReadingsAsync(5);

        // Assert
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSensorData()
    {
        // Arrange
        var room = new Room { RoomNumber = "206", Floor = "2", Capacity = 1 };
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        var sensorData = await _repository.CreateAsync(new SensorData 
        { 
            RoomId = room.Id, 
            Temperature = 20.0M, 
            Humidity = 40.0M, 
            Timestamp = DateTime.UtcNow 
        });

        // Act
        var result = await _repository.DeleteAsync(sensorData.Id);

        // Assert
        result.Should().BeTrue();
        var deleted = await _repository.GetByIdAsync(sensorData.Id);
        deleted.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

