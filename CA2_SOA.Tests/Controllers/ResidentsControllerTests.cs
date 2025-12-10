using CA2_SOA.Controllers;
using CA2_SOA.DTOs;
using CA2_SOA.Interfaces;
using CA2_SOA.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CA2_SOA.Tests.Controllers;

public class ResidentsControllerTests
{
    private readonly Mock<IResidentRepository> _mockResidentRepository;
    private readonly ResidentsController _controller;

    public ResidentsControllerTests()
    {
        _mockResidentRepository = new Mock<IResidentRepository>();
        _controller = new ResidentsController(_mockResidentRepository.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResultWithResidents()
    {
        // Arrange
        var residents = new List<Resident>
        {
            new Resident
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1940, 1, 1),
                MedicalConditions = "Diabetes",
                EmergencyContact = "Jane Doe",
                EmergencyPhone = "123-456-7890",
                AdmissionDate = DateTime.UtcNow.AddDays(-30),
                IsActive = true,
                RoomId = 1,
                Room = new Room { RoomNumber = "101" }
            },
            new Resident
            {
                Id = 2,
                FirstName = "Mary",
                LastName = "Smith",
                DateOfBirth = new DateTime(1945, 5, 15),
                MedicalConditions = "Hypertension",
                EmergencyContact = "Bob Smith",
                EmergencyPhone = "098-765-4321",
                AdmissionDate = DateTime.UtcNow.AddDays(-60),
                IsActive = true,
                RoomId = 2,
                Room = new Room { RoomNumber = "102" }
            }
        };

        _mockResidentRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(residents);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedResidents = okResult!.Value as IEnumerable<ResidentDto>;
        returnedResidents.Should().HaveCount(2);
        returnedResidents!.First().FirstName.Should().Be("John");
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOkResultWithResident()
    {
        // Arrange
        var resident = new Resident
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1940, 1, 1),
            MedicalConditions = "Diabetes",
            EmergencyContact = "Jane Doe",
            EmergencyPhone = "123-456-7890",
            AdmissionDate = DateTime.UtcNow,
            IsActive = true,
            RoomId = 1,
            Room = new Room { RoomNumber = "101" }
        };

        _mockResidentRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(resident);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedResident = okResult!.Value as ResidentDto;
        returnedResident.Should().NotBeNull();
        returnedResident!.FirstName.Should().Be("John");
        returnedResident.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockResidentRepository.Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((Resident?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateResidentDto(
            FirstName: "Alice",
            LastName: "Johnson",
            DateOfBirth: new DateTime(1950, 3, 10),
            MedicalConditions: "Arthritis",
            EmergencyContact: "Bob Johnson",
            EmergencyPhone: "555-0123",
            RoomId: 3
        );

        var createdResident = new Resident
        {
            Id = 3,
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            DateOfBirth = createDto.DateOfBirth,
            MedicalConditions = createDto.MedicalConditions,
            EmergencyContact = createDto.EmergencyContact,
            EmergencyPhone = createDto.EmergencyPhone,
            RoomId = createDto.RoomId,
            AdmissionDate = DateTime.UtcNow,
            IsActive = true
        };

        _mockResidentRepository.Setup(repo => repo.CreateAsync(It.IsAny<Resident>()))
            .ReturnsAsync(createdResident);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        var returnedResident = createdResult!.Value as ResidentDto;
        returnedResident.Should().NotBeNull();
        returnedResident!.FirstName.Should().Be("Alice");
        returnedResident.LastName.Should().Be("Johnson");
    }

    [Fact]
    public async Task Update_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var updateDto = new UpdateResidentDto(
            FirstName: "John",
            LastName: "Doe-Updated",
            DateOfBirth: new DateTime(1940, 1, 1),
            MedicalConditions: "Diabetes, Hypertension",
            EmergencyContact: "Jane Doe",
            EmergencyPhone: "123-456-7890",
            IsActive: true,
            RoomId: 2
        );

        var existingResident = new Resident
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1940, 1, 1),
            MedicalConditions = "Diabetes",
            EmergencyContact = "Jane Doe",
            EmergencyPhone = "123-456-7890",
            AdmissionDate = DateTime.UtcNow.AddDays(-30),
            IsActive = true,
            RoomId = 1,
            Room = new Room { RoomNumber = "101" }
        };

        var updatedResident = new Resident
        {
            Id = 1,
            FirstName = updateDto.FirstName!,
            LastName = updateDto.LastName!,
            DateOfBirth = updateDto.DateOfBirth!.Value,
            MedicalConditions = updateDto.MedicalConditions,
            EmergencyContact = updateDto.EmergencyContact,
            EmergencyPhone = updateDto.EmergencyPhone,
            AdmissionDate = existingResident.AdmissionDate,
            IsActive = updateDto.IsActive!.Value,
            RoomId = updateDto.RoomId!.Value,
            Room = new Room { RoomNumber = "102" }
        };

        _mockResidentRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(existingResident);
        _mockResidentRepository.Setup(repo => repo.UpdateAsync(1, It.IsAny<Resident>()))
            .ReturnsAsync(updatedResident);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedResident = okResult!.Value as ResidentDto;
        returnedResident.Should().NotBeNull();
        returnedResident!.LastName.Should().Be("Doe-Updated");
        returnedResident.RoomNumber.Should().Be("102");
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateResidentDto(
            FirstName: "Test",
            LastName: null,
            DateOfBirth: null,
            MedicalConditions: null,
            EmergencyContact: null,
            EmergencyPhone: null,
            IsActive: null,
            RoomId: null
        );

        _mockResidentRepository.Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((Resident?)null);

        // Act
        var result = await _controller.Update(999, updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockResidentRepository.Setup(repo => repo.DeleteAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockResidentRepository.Setup(repo => repo.DeleteAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetActive_ReturnsOnlyActiveResidents()
    {
        // Arrange
        var activeResidents = new List<Resident>
        {
            new Resident
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1940, 1, 1),
                AdmissionDate = DateTime.UtcNow,
                IsActive = true,
                Room = new Room { RoomNumber = "101" }
            }
        };

        _mockResidentRepository.Setup(repo => repo.GetActiveResidentsAsync())
            .ReturnsAsync(activeResidents);

        // Act
        var result = await _controller.GetActive();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedResidents = okResult!.Value as IEnumerable<ResidentDto>;
        returnedResidents.Should().HaveCount(1);
        returnedResidents!.First().IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetByRoom_ReturnsResidentsInSpecificRoom()
    {
        // Arrange
        var roomResidents = new List<Resident>
        {
            new Resident
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1940, 1, 1),
                AdmissionDate = DateTime.UtcNow,
                IsActive = true,
                RoomId = 1,
                Room = new Room { RoomNumber = "101" }
            },
            new Resident
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Doe",
                DateOfBirth = new DateTime(1942, 5, 5),
                AdmissionDate = DateTime.UtcNow,
                IsActive = true,
                RoomId = 1,
                Room = new Room { RoomNumber = "101" }
            }
        };

        _mockResidentRepository.Setup(repo => repo.GetResidentsByRoomAsync(1))
            .ReturnsAsync(roomResidents);

        // Act
        var result = await _controller.GetByRoom(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedResidents = okResult!.Value as IEnumerable<ResidentDto>;
        returnedResidents.Should().HaveCount(2);
        returnedResidents!.All(r => r.RoomNumber == "101").Should().BeTrue();
    }
}

