using CA2_SOA.Data;
using CA2_SOA.Models;
using CA2_SOA.Repositories;
using CA2_SOA.Tests.Helpers;
using FluentAssertions;

namespace CA2_SOA.Tests.Repositories;

public class AlertRepositoryTests
{
    private readonly CareHomeDbContext _context;
    private readonly AlertRepository _repository;

    public AlertRepositoryTests()
    {
        _context = DbContextHelper.CreateInMemoryContext();
        _repository = new AlertRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllAlerts()
    {
        // Arrange
        var room = new Room { RoomNumber = "101", Floor = "1", Capacity = 2, CreatedAt = DateTime.UtcNow };
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        var alerts = new List<Alert>
        {
            new Alert
            {
                RoomId = room.Id,
                AlertType = "Temperature",
                Severity = "High",
                Message = "Temperature too high",
                CreatedAt = DateTime.UtcNow,
                IsResolved = false
            },
            new Alert
            {
                RoomId = room.Id,
                AlertType = "Humidity",
                Severity = "Medium",
                Message = "Humidity abnormal",
                CreatedAt = DateTime.UtcNow,
                IsResolved = false
            }
        };
        await _context.Alerts.AddRangeAsync(alerts);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(a => a.AlertType == "Temperature");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsAlert()
    {
        // Arrange
        var room = new Room { RoomNumber = "101", Floor = "1", Capacity = 2, CreatedAt = DateTime.UtcNow };
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        var alert = new Alert
        {
            RoomId = room.Id,
            AlertType = "Temperature",
            Severity = "Critical",
            Message = "Critical temperature",
            CreatedAt = DateTime.UtcNow,
            IsResolved = false
        };
        await _context.Alerts.AddAsync(alert);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(alert.Id);

        // Assert
        result.Should().NotBeNull();
        result!.AlertType.Should().Be("Temperature");
        result.Severity.Should().Be("Critical");
    }

    [Fact]
    public async Task CreateAsync_AddsAlertToDatabase()
    {
        // Arrange
        var room = new Room { RoomNumber = "101", Floor = "1", Capacity = 2, CreatedAt = DateTime.UtcNow };
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        var alert = new Alert
        {
            RoomId = room.Id,
            AlertType = "Motion",
            Severity = "Low",
            Message = "Motion detected",
            CreatedAt = DateTime.UtcNow,
            IsResolved = false
        };

        // Act
        var result = await _repository.CreateAsync(alert);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        var savedAlert = await _context.Alerts.FindAsync(result.Id);
        savedAlert.Should().NotBeNull();
        savedAlert!.AlertType.Should().Be("Motion");
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingAlert()
    {
        // Arrange
        var room = new Room { RoomNumber = "101", Floor = "1", Capacity = 2, CreatedAt = DateTime.UtcNow };
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        var alert = new Alert
        {
            RoomId = room.Id,
            AlertType = "Temperature",
            Severity = "High",
            Message = "Temperature too high",
            CreatedAt = DateTime.UtcNow,
            IsResolved = false
        };
        await _context.Alerts.AddAsync(alert);
        await _context.SaveChangesAsync();

        alert.IsResolved = true;
        alert.ResolvedAt = DateTime.UtcNow;
        alert.ResolvedByUserId = 1;
        alert.ResolutionNotes = "Fixed HVAC system";

        // Act
        var result = await _repository.UpdateAsync(alert.Id, alert);

        // Assert
        result.Should().NotBeNull();
        result!.IsResolved.Should().BeTrue();
        result.ResolutionNotes.Should().Be("Fixed HVAC system");
    }

    [Fact]
    public async Task DeleteAsync_RemovesAlertFromDatabase()
    {
        // Arrange
        var room = new Room { RoomNumber = "101", Floor = "1", Capacity = 2, CreatedAt = DateTime.UtcNow };
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        var alert = new Alert
        {
            RoomId = room.Id,
            AlertType = "Temperature",
            Severity = "High",
            Message = "Alert",
            CreatedAt = DateTime.UtcNow,
            IsResolved = false
        };
        await _context.Alerts.AddAsync(alert);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(alert.Id);

        // Assert
        result.Should().BeTrue();
        var deletedAlert = await _context.Alerts.FindAsync(alert.Id);
        deletedAlert.Should().BeNull();
    }

    [Fact]
    public async Task GetActiveAlertsAsync_ReturnsOnlyUnresolvedAlerts()
    {
        // Arrange
        var room = new Room { RoomNumber = "101", Floor = "1", Capacity = 2, CreatedAt = DateTime.UtcNow };
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();

        var alerts = new List<Alert>
        {
            new Alert
            {
                RoomId = room.Id,
                AlertType = "Temperature",
                Severity = "High",
                Message = "Active alert",
                CreatedAt = DateTime.UtcNow,
                IsResolved = false
            },
            new Alert
            {
                RoomId = room.Id,
                AlertType = "Humidity",
                Severity = "Medium",
                Message = "Resolved alert",
                CreatedAt = DateTime.UtcNow,
                IsResolved = true,
                ResolvedAt = DateTime.UtcNow
            }
        };
        await _context.Alerts.AddRangeAsync(alerts);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActiveAlertsAsync();

        // Assert
        result.Should().HaveCount(1);
        result.Should().OnlyContain(a => !a.IsResolved);
        result.First().Message.Should().Be("Active alert");
    }

    [Fact]
    public async Task GetAlertsByRoomAsync_ReturnsAlertsForSpecificRoom()
    {
        // Arrange
        var room1 = new Room { RoomNumber = "101", Floor = "1", Capacity = 2, CreatedAt = DateTime.UtcNow };
        var room2 = new Room { RoomNumber = "102", Floor = "1", Capacity = 1, CreatedAt = DateTime.UtcNow };
        await _context.Rooms.AddRangeAsync(room1, room2);
        await _context.SaveChangesAsync();

        var alerts = new List<Alert>
        {
            new Alert
            {
                RoomId = room1.Id,
                AlertType = "Temperature",
                Severity = "High",
                Message = "Alert 1",
                CreatedAt = DateTime.UtcNow,
                IsResolved = false
            },
            new Alert
            {
                RoomId = room1.Id,
                AlertType = "Humidity",
                Severity = "Medium",
                Message = "Alert 2",
                CreatedAt = DateTime.UtcNow,
                IsResolved = false
            },
            new Alert
            {
                RoomId = room2.Id,
                AlertType = "Motion",
                Severity = "Low",
                Message = "Alert 3",
                CreatedAt = DateTime.UtcNow,
                IsResolved = false
            }
        };
        await _context.Alerts.AddRangeAsync(alerts);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAlertsByRoomAsync(room1.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(a => a.RoomId == room1.Id);
    }

}

