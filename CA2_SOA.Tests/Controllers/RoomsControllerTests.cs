using CA2_SOA.Controllers;
using CA2_SOA.DTOs;
using CA2_SOA.Interfaces;
using CA2_SOA.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CA2_SOA.Tests.Controllers;

public class RoomsControllerTests
{
    private readonly Mock<IRoomRepository> _mockRoomRepository;
    private readonly RoomsController _controller;

    public RoomsControllerTests()
    {
        _mockRoomRepository = new Mock<IRoomRepository>();
        _controller = new RoomsController(_mockRoomRepository.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResultWithRooms()
    {
        // Arrange
        var rooms = new List<Room>
        {
            new Room
            {
                Id = 1,
                RoomNumber = "101",
                RoomName = "Rose Room",
                Floor = "1",
                Capacity = 2,
                IsOccupied = true,
                CreatedAt = DateTime.UtcNow,
                Residents = new List<Resident>
                {
                    new Resident { Id = 1, FirstName = "John", LastName = "Doe" }
                },
                SensorReadings = new List<SensorData>(),
                Alerts = new List<Alert>()
            },
            new Room
            {
                Id = 2,
                RoomNumber = "102",
                RoomName = "Lily Room",
                Floor = "1",
                Capacity = 1,
                IsOccupied = false,
                CreatedAt = DateTime.UtcNow,
                Residents = new List<Resident>(),
                SensorReadings = new List<SensorData>(),
                Alerts = new List<Alert>()
            }
        };

        _mockRoomRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(rooms);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedRooms = okResult!.Value as IEnumerable<RoomDto>;
        returnedRooms.Should().HaveCount(2);
        returnedRooms!.First().RoomNumber.Should().Be("101");
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOkResultWithRoom()
    {
        // Arrange
        var room = new Room
        {
            Id = 1,
            RoomNumber = "101",
            RoomName = "Rose Room",
            Floor = "1",
            Capacity = 2,
            IsOccupied = true,
            CreatedAt = DateTime.UtcNow,
            Residents = new List<Resident>(),
            SensorReadings = new List<SensorData>(),
            Alerts = new List<Alert>()
        };

        _mockRoomRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(room);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedRoom = okResult!.Value as RoomDto;
        returnedRoom.Should().NotBeNull();
        returnedRoom!.RoomNumber.Should().Be("101");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockRoomRepository.Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((Room?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateRoomDto(
            RoomNumber: "103",
            RoomName: "Orchid Room",
            Floor: "2",
            Capacity: 1,
            Notes: "Test notes"
        );

        var createdRoom = new Room
        {
            Id = 3,
            RoomNumber = createDto.RoomNumber,
            RoomName = createDto.RoomName,
            Floor = createDto.Floor,
            Capacity = createDto.Capacity,
            Notes = createDto.Notes,
            CreatedAt = DateTime.UtcNow,
            Residents = new List<Resident>(),
            SensorReadings = new List<SensorData>(),
            Alerts = new List<Alert>()
        };

        _mockRoomRepository.Setup(repo => repo.RoomNumberExistsAsync(createDto.RoomNumber))
            .ReturnsAsync(false);
        _mockRoomRepository.Setup(repo => repo.CreateAsync(It.IsAny<Room>()))
            .ReturnsAsync(createdRoom);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        var returnedRoom = createdResult!.Value as RoomDto;
        returnedRoom.Should().NotBeNull();
        returnedRoom!.RoomNumber.Should().Be("103");
    }

    [Fact]
    public async Task Create_WithDuplicateRoomNumber_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateRoomDto(
            RoomNumber: "101",
            RoomName: "Duplicate Room",
            Floor: "1",
            Capacity: 1,
            Notes: null
        );

        _mockRoomRepository.Setup(repo => repo.RoomNumberExistsAsync(createDto.RoomNumber))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var updateDto = new UpdateRoomDto(
            RoomNumber: "101-Updated",
            RoomName: "Updated Room",
            Floor: "1",
            Capacity: 3,
            IsOccupied: true,
            Notes: "Updated notes"
        );

        var existingRoom = new Room
        {
            Id = 1,
            RoomNumber = "101",
            RoomName = "Rose Room",
            Floor = "1",
            Capacity = 2,
            IsOccupied = false,
            CreatedAt = DateTime.UtcNow,
            Residents = new List<Resident>(),
            SensorReadings = new List<SensorData>(),
            Alerts = new List<Alert>()
        };

        var updatedRoom = new Room
        {
            Id = 1,
            RoomNumber = updateDto.RoomNumber!,
            RoomName = updateDto.RoomName,
            Floor = updateDto.Floor!,
            Capacity = updateDto.Capacity!.Value,
            IsOccupied = updateDto.IsOccupied!.Value,
            Notes = updateDto.Notes,
            CreatedAt = existingRoom.CreatedAt,
            Residents = new List<Resident>(),
            SensorReadings = new List<SensorData>(),
            Alerts = new List<Alert>()
        };

        _mockRoomRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(existingRoom);
        _mockRoomRepository.Setup(repo => repo.UpdateAsync(1, It.IsAny<Room>()))
            .ReturnsAsync(updatedRoom);

        // Act
        var result = await _controller.Update(1, updateDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedRoom = okResult!.Value as RoomDto;
        returnedRoom.Should().NotBeNull();
        returnedRoom!.RoomNumber.Should().Be("101-Updated");
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateRoomDto(
            RoomNumber: "999",
            RoomName: null,
            Floor: null,
            Capacity: null,
            IsOccupied: null,
            Notes: null
        );

        _mockRoomRepository.Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((Room?)null);

        // Act
        var result = await _controller.Update(999, updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockRoomRepository.Setup(repo => repo.DeleteAsync(1))
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
        _mockRoomRepository.Setup(repo => repo.DeleteAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetOccupied_ReturnsOnlyOccupiedRooms()
    {
        // Arrange
        var occupiedRooms = new List<Room>
        {
            new Room
            {
                Id = 1,
                RoomNumber = "101",
                IsOccupied = true,
                CreatedAt = DateTime.UtcNow,
                Residents = new List<Resident> { new Resident() },
                SensorReadings = new List<SensorData>(),
                Alerts = new List<Alert>()
            }
        };

        _mockRoomRepository.Setup(repo => repo.GetOccupiedRoomsAsync())
            .ReturnsAsync(occupiedRooms);

        // Act
        var result = await _controller.GetOccupied();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedRooms = okResult!.Value as IEnumerable<RoomDto>;
        returnedRooms.Should().HaveCount(1);
        returnedRooms!.First().IsOccupied.Should().BeTrue();
    }

    [Fact]
    public async Task GetAvailable_ReturnsOnlyAvailableRooms()
    {
        // Arrange
        var availableRooms = new List<Room>
        {
            new Room
            {
                Id = 2,
                RoomNumber = "102",
                IsOccupied = false,
                CreatedAt = DateTime.UtcNow,
                Residents = new List<Resident>(),
                SensorReadings = new List<SensorData>(),
                Alerts = new List<Alert>()
            }
        };

        _mockRoomRepository.Setup(repo => repo.GetAvailableRoomsAsync())
            .ReturnsAsync(availableRooms);

        // Act
        var result = await _controller.GetAvailable();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedRooms = okResult!.Value as IEnumerable<RoomDto>;
        returnedRooms.Should().HaveCount(1);
        returnedRooms!.First().IsOccupied.Should().BeFalse();
    }
}

