using CA2_SOA.Controllers;
using CA2_SOA.DTOs;
using CA2_SOA.Interfaces;
using CA2_SOA.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CA2_SOA.Tests.Controllers;

public class AlertsControllerTests
{
    private readonly Mock<IAlertRepository> _mockAlertRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly AlertsController _controller;

    public AlertsControllerTests()
    {
        _mockAlertRepository = new Mock<IAlertRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _controller = new AlertsController(_mockAlertRepository.Object, _mockUserRepository.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResultWithAlerts()
    {
        // Arrange
        var alerts = new List<Alert>
        {
            new Alert
            {
                Id = 1,
                RoomId = 1,
                Room = new Room { RoomNumber = "101" },
                AlertType = "Temperature",
                Severity = "High",
                Message = "Temperature too high",
                CreatedAt = DateTime.UtcNow,
                IsResolved = false,
                ResolvedByUserId = null
            },
            new Alert
            {
                Id = 2,
                RoomId = 2,
                Room = new Room { RoomNumber = "102" },
                AlertType = "Humidity",
                Severity = "Medium",
                Message = "Humidity levels abnormal",
                CreatedAt = DateTime.UtcNow,
                IsResolved = true,
                ResolvedAt = DateTime.UtcNow,
                ResolvedByUserId = 1,
                ResolutionNotes = "Fixed HVAC system"
            }
        };

        _mockAlertRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(alerts);
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Username = "admin" });

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedAlerts = okResult!.Value as IEnumerable<AlertDto>;
        returnedAlerts.Should().HaveCount(2);
        returnedAlerts!.First().AlertType.Should().Be("Temperature");
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsOkResultWithAlert()
    {
        // Arrange
        var alert = new Alert
        {
            Id = 1,
            RoomId = 1,
            Room = new Room { RoomNumber = "101" },
            AlertType = "Temperature",
            Severity = "Critical",
            Message = "Temperature critical",
            CreatedAt = DateTime.UtcNow,
            IsResolved = false
        };

        _mockAlertRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(alert);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedAlert = okResult!.Value as AlertDto;
        returnedAlert.Should().NotBeNull();
        returnedAlert!.Severity.Should().Be("Critical");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockAlertRepository.Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((Alert?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetActive_ReturnsOnlyUnresolvedAlerts()
    {
        // Arrange
        var activeAlerts = new List<Alert>
        {
            new Alert
            {
                Id = 1,
                RoomId = 1,
                Room = new Room { RoomNumber = "101" },
                AlertType = "Temperature",
                Severity = "High",
                Message = "Temperature too high",
                CreatedAt = DateTime.UtcNow,
                IsResolved = false
            }
        };

        _mockAlertRepository.Setup(repo => repo.GetActiveAlertsAsync())
            .ReturnsAsync(activeAlerts);

        // Act
        var result = await _controller.GetActive();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedAlerts = okResult!.Value as IEnumerable<AlertDto>;
        returnedAlerts.Should().HaveCount(1);
        returnedAlerts!.First().IsResolved.Should().BeFalse();
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateAlertDto(
            RoomId: 1,
            AlertType: "Motion",
            Severity: "Low",
            Message: "Motion detected"
        );

        var createdAlert = new Alert
        {
            Id = 3,
            RoomId = createDto.RoomId,
            Room = new Room { RoomNumber = "103" },
            AlertType = createDto.AlertType,
            Severity = createDto.Severity,
            Message = createDto.Message,
            CreatedAt = DateTime.UtcNow,
            IsResolved = false
        };

        _mockAlertRepository.Setup(repo => repo.CreateAsync(It.IsAny<Alert>()))
            .ReturnsAsync(createdAlert);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        var returnedAlert = createdResult!.Value as AlertDto;
        returnedAlert.Should().NotBeNull();
        returnedAlert!.AlertType.Should().Be("Motion");
    }

    [Fact]
    public async Task Resolve_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var resolveDto = new ResolveAlertDto(
            UserId: 1,
            ResolutionNotes: "Fixed the issue"
        );

        var existingAlert = new Alert
        {
            Id = 1,
            RoomId = 1,
            Room = new Room { RoomNumber = "101" },
            AlertType = "Temperature",
            Severity = "High",
            Message = "Temperature too high",
            CreatedAt = DateTime.UtcNow,
            IsResolved = false
        };

        var resolvedAlert = new Alert
        {
            Id = 1,
            RoomId = 1,
            Room = new Room { RoomNumber = "101" },
            AlertType = "Temperature",
            Severity = "High",
            Message = "Temperature too high",
            CreatedAt = existingAlert.CreatedAt,
            IsResolved = true,
            ResolvedAt = DateTime.UtcNow,
            ResolvedByUserId = resolveDto.UserId,
            ResolutionNotes = resolveDto.ResolutionNotes
        };

        var user = new User { Id = 1, Username = "admin" };

        _mockAlertRepository.Setup(repo => repo.ResolveAlertAsync(1, resolveDto.UserId, resolveDto.ResolutionNotes))
            .ReturnsAsync(resolvedAlert);
        _mockAlertRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(resolvedAlert);
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Resolve(1, resolveDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedAlert = okResult!.Value as AlertDto;
        returnedAlert.Should().NotBeNull();
        returnedAlert!.IsResolved.Should().BeTrue();
        returnedAlert.ResolutionNotes.Should().Be("Fixed the issue");
    }

    [Fact]
    public async Task Resolve_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var resolveDto = new ResolveAlertDto(
            UserId: 1,
            ResolutionNotes: "Notes"
        );

        _mockAlertRepository.Setup(repo => repo.ResolveAlertAsync(999, resolveDto.UserId, resolveDto.ResolutionNotes))
            .ReturnsAsync((Alert?)null);

        // Act
        var result = await _controller.Resolve(999, resolveDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockAlertRepository.Setup(repo => repo.DeleteAsync(1))
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
        _mockAlertRepository.Setup(repo => repo.DeleteAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetByRoom_ReturnsAlertsForSpecificRoom()
    {
        // Arrange
        var roomAlerts = new List<Alert>
        {
            new Alert
            {
                Id = 1,
                RoomId = 1,
                Room = new Room { RoomNumber = "101" },
                AlertType = "Temperature",
                Severity = "High",
                Message = "Alert 1",
                CreatedAt = DateTime.UtcNow,
                IsResolved = false
            },
            new Alert
            {
                Id = 2,
                RoomId = 1,
                Room = new Room { RoomNumber = "101" },
                AlertType = "Humidity",
                Severity = "Medium",
                Message = "Alert 2",
                CreatedAt = DateTime.UtcNow,
                IsResolved = false
            }
        };

        _mockAlertRepository.Setup(repo => repo.GetAlertsByRoomAsync(1))
            .ReturnsAsync(roomAlerts);

        // Act
        var result = await _controller.GetByRoom(1);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedAlerts = okResult!.Value as IEnumerable<AlertDto>;
        returnedAlerts.Should().HaveCount(2);
        returnedAlerts!.All(a => a.RoomNumber == "101").Should().BeTrue();
    }
}

