using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using CA2_SOA.Models;
using CA2_SOA.Services;
using System.Collections.Generic;

namespace CA2_SOA.Tests.Services;

/// <summary>
/// Unit tests for JwtService
/// Tests token generation and validation
/// </summary>
public class JwtServiceTests
{
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"Jwt:Key", "YourSuperSecretKeyThatIsAtLeast32CharactersLongForTesting!"},
            {"Jwt:Issuer", "CareHomeAPI"},
            {"Jwt:Audience", "CareHomeClient"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _jwtService = new JwtService(configuration);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidToken()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            FullName = "Test User",
            Role = "User",
            PasswordHash = "hashedpassword",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        // Act
        var token = _jwtService.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3);
    }

    [Fact]
    public void GenerateToken_ShouldIncludeUserClaims()
    {
        // Arrange
        var user = new User
        {
            Id = 42,
            Username = "johndoe",
            Email = "john@example.com",
            FullName = "John Doe",
            Role = "Admin",
            PasswordHash = "hashedpassword",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        // Act
        var token = _jwtService.GenerateToken(user);
        var principal = _jwtService.ValidateToken(token);

        // Assert
        principal.Should().NotBeNull();
        principal!.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value.Should().Be("42");
        principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value.Should().Be("johndoe");
        principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value.Should().Be("john@example.com");
        principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value.Should().Be("Admin");
    }

    [Fact]
    public void ValidateToken_ShouldReturnNull_ForInvalidToken()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var result = _jwtService.ValidateToken(invalidToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_ShouldSucceed_ForValidToken()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "validuser",
            Email = "valid@example.com",
            FullName = "Valid User",
            Role = "User",
            PasswordHash = "hashedpassword",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var token = _jwtService.GenerateToken(user);

        // Act
        var principal = _jwtService.ValidateToken(token);

        // Assert
        principal.Should().NotBeNull();
        principal!.Identity.Should().NotBeNull();
        principal.Identity!.IsAuthenticated.Should().BeTrue();
    }
}

