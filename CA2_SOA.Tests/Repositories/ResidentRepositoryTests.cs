using CA2_SOA.Data;
using CA2_SOA.Models;
using CA2_SOA.Repositories;
using CA2_SOA.Tests.Helpers;
using FluentAssertions;

namespace CA2_SOA.Tests.Repositories;

public class ResidentRepositoryTests
{
    private readonly CareHomeDbContext _context;
    private readonly ResidentRepository _repository;

    public ResidentRepositoryTests()
    {
        _context = DbContextHelper.CreateInMemoryContext();
        _repository = new ResidentRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllResidents()
    {
        // Arrange
        var residents = new List<Resident>
        {
            new Resident
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1940, 1, 1),
                AdmissionDate = DateTime.UtcNow,
                IsActive = true
            },
            new Resident
            {
                FirstName = "Jane",
                LastName = "Smith",
                DateOfBirth = new DateTime(1945, 5, 5),
                AdmissionDate = DateTime.UtcNow,
                IsActive = true
            }
        };
        await _context.Residents.AddRangeAsync(residents);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.FirstName == "John");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsResident()
    {
        // Arrange
        var resident = new Resident
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1940, 1, 1),
            MedicalConditions = "Diabetes",
            AdmissionDate = DateTime.UtcNow,
            IsActive = true
        };
        await _context.Residents.AddAsync(resident);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(resident.Id);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("John");
        result.MedicalConditions.Should().Be("Diabetes");
    }

    [Fact]
    public async Task CreateAsync_AddsResidentToDatabase()
    {
        // Arrange
        var resident = new Resident
        {
            FirstName = "Alice",
            LastName = "Johnson",
            DateOfBirth = new DateTime(1950, 3, 10),
            MedicalConditions = "Arthritis",
            EmergencyContact = "Bob Johnson",
            EmergencyPhone = "555-0123",
            AdmissionDate = DateTime.UtcNow,
            IsActive = true
        };

        // Act
        var result = await _repository.CreateAsync(resident);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        var savedResident = await _context.Residents.FindAsync(result.Id);
        savedResident.Should().NotBeNull();
        savedResident!.FirstName.Should().Be("Alice");
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingResident()
    {
        // Arrange
        var resident = new Resident
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1940, 1, 1),
            AdmissionDate = DateTime.UtcNow,
            IsActive = true
        };
        await _context.Residents.AddAsync(resident);
        await _context.SaveChangesAsync();

        resident.MedicalConditions = "Diabetes, Hypertension";
        resident.EmergencyContact = "Jane Doe";

        // Act
        var result = await _repository.UpdateAsync(resident.Id, resident);

        // Assert
        result.Should().NotBeNull();
        result!.MedicalConditions.Should().Be("Diabetes, Hypertension");
        result.EmergencyContact.Should().Be("Jane Doe");
    }

    [Fact]
    public async Task DeleteAsync_RemovesResidentFromDatabase()
    {
        // Arrange
        var resident = new Resident
        {
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1940, 1, 1),
            AdmissionDate = DateTime.UtcNow,
            IsActive = true
        };
        await _context.Residents.AddAsync(resident);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(resident.Id);

        // Assert
        result.Should().BeTrue();
        var deletedResident = await _context.Residents.FindAsync(resident.Id);
        deletedResident.Should().BeNull();
    }

    [Fact]
    public async Task GetActiveResidentsAsync_ReturnsOnlyActiveResidents()
    {
        // Arrange
        var residents = new List<Resident>
        {
            new Resident
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1940, 1, 1),
                AdmissionDate = DateTime.UtcNow,
                IsActive = true
            },
            new Resident
            {
                FirstName = "Jane",
                LastName = "Smith",
                DateOfBirth = new DateTime(1945, 5, 5),
                AdmissionDate = DateTime.UtcNow,
                IsActive = false
            }
        };
        await _context.Residents.AddRangeAsync(residents);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActiveResidentsAsync();

        // Assert
        result.Should().HaveCount(1);
        result.Should().OnlyContain(r => r.IsActive);
        result.First().FirstName.Should().Be("John");
    }

    [Fact]
    public async Task GetResidentsByRoomAsync_ReturnsResidentsInSpecificRoom()
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

        var residents = new List<Resident>
        {
            new Resident
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1940, 1, 1),
                AdmissionDate = DateTime.UtcNow,
                IsActive = true,
                RoomId = room.Id
            },
            new Resident
            {
                FirstName = "Jane",
                LastName = "Doe",
                DateOfBirth = new DateTime(1942, 5, 5),
                AdmissionDate = DateTime.UtcNow,
                IsActive = true,
                RoomId = room.Id
            },
            new Resident
            {
                FirstName = "Bob",
                LastName = "Smith",
                DateOfBirth = new DateTime(1945, 3, 3),
                AdmissionDate = DateTime.UtcNow,
                IsActive = true,
                RoomId = null
            }
        };
        await _context.Residents.AddRangeAsync(residents);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetResidentsByRoomAsync(room.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(r => r.RoomId == room.Id);
    }
}

