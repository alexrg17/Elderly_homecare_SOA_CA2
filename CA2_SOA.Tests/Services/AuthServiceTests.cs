using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Configuration;
using CA2_SOA.Models;
using CA2_SOA.DTOs;
using CA2_SOA.Services;
using CA2_SOA.Interfaces;
using System.Collections.Generic;

namespace CA2_SOA.Tests.Services;

/// <summary>
/// Unit tests for AuthService
/// Tests login and registration logic with mocked dependencies
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly JwtService _jwtService;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        
        // Create real JwtService with test configuration
        var inMemorySettings = new Dictionary<string, string>
        {
            {"Jwt:Key", "TestSecretKeyThatIsAtLeast32CharactersLong!"},
            {"Jwt:Issuer", "TestIssuer"},
            {"Jwt:Audience", "TestAudience"}
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
        _jwtService = new JwtService(configuration);
        
        _authService = new AuthService(_mockUserRepository.Object, _jwtService);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var loginDto = new LoginDto("nonexistent", "password");
        _mockUserRepository.Setup(r => r.GetByUsernameAsync("nonexistent"))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenUserIsInactive()
    {
        // Arrange
        var loginDto = new LoginDto("inactiveuser", "password");
        var inactiveUser = new User
        {
            Id = 1,
            Username = "inactiveuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            FullName = "Inactive User",
            Email = "inactive@example.com",
            Role = "User",
            CreatedAt = DateTime.UtcNow,
            IsActive = false
        };

        _mockUserRepository.Setup(r => r.GetByUsernameAsync("inactiveuser"))
            .ReturnsAsync(inactiveUser);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnLoginResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var loginDto = new LoginDto("testuser", "password123");
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FullName = "Test User",
            Email = "test@example.com",
            Role = "User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _mockUserRepository.Setup(r => r.GetByUsernameAsync("testuser"))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.User.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnNull_WhenUsernameAlreadyExists()
    {
        // Arrange
        var createUserDto = new CreateUserDto(
            "existinguser",
            "password123",
            "Existing User",
            "existing@example.com",
            "User"
        );

        _mockUserRepository.Setup(r => r.UsernameExistsAsync("existinguser"))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterAsync(createUserDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WhenDataIsValid()
    {
        // Arrange
        var createUserDto = new CreateUserDto(
            "newuser",
            "password123",
            "New User",
            "new@example.com",
            "User"
        );

        _mockUserRepository.Setup(r => r.UsernameExistsAsync("newuser"))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.EmailExistsAsync("new@example.com"))
            .ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => { u.Id = 1; return u; });

        // Act
        var result = await _authService.RegisterAsync(createUserDto);

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("newuser");
        result.Email.Should().Be("new@example.com");
    }
}

