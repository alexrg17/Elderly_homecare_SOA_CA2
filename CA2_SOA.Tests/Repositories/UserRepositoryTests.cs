using Xunit;
using FluentAssertions;
using CA2_SOA.Models;
using CA2_SOA.Repositories;
using CA2_SOA.Tests.Helpers;

namespace CA2_SOA.Tests.Repositories;

/// <summary>
/// Unit tests for UserRepository
/// Tests CRUD operations and username/email validation
/// </summary>
public class UserRepositoryTests : IDisposable
{
    private readonly UserRepository _repository;
    private readonly Data.CareHomeDbContext _context;

    public UserRepositoryTests()
    {
        _context = DbContextHelper.CreateInMemoryContext();
        _repository = new UserRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUser()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FullName = "Test User",
            Email = "test@example.com",
            Role = "User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        // Act
        var result = await _repository.CreateAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Username.Should().Be("testuser");
        result.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FullName = "Test User",
            Email = "test@example.com",
            Role = "User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        var created = await _repository.CreateAsync(user);

        // Act
        var result = await _repository.GetByIdAsync(created.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
        result.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task GetByUsernameAsync_ShouldReturnUser_WhenUsernameExists()
    {
        // Arrange
        var user = new User
        {
            Username = "uniqueuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FullName = "Unique User",
            Email = "unique@example.com",
            Role = "User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        await _repository.CreateAsync(user);

        // Act
        var result = await _repository.GetByUsernameAsync("uniqueuser");

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("uniqueuser");
    }

    [Fact]
    public async Task UsernameExistsAsync_ShouldReturnTrue_WhenUsernameExists()
    {
        // Arrange
        await _repository.CreateAsync(new User
        {
            Username = "existinguser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FullName = "Existing User",
            Email = "existing@example.com",
            Role = "User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        });

        // Act
        var result = await _repository.UsernameExistsAsync("existinguser");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldReturnTrue_WhenEmailExists()
    {
        // Arrange
        await _repository.CreateAsync(new User
        {
            Username = "emailcheck",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FullName = "Email Check",
            Email = "exists@example.com",
            Role = "User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        });

        // Act
        var result = await _repository.EmailExistsAsync("exists@example.com");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        var user = new User
        {
            Username = "deleteuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FullName = "Delete User",
            Email = "delete@example.com",
            Role = "User",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        var created = await _repository.CreateAsync(user);

        // Act
        var result = await _repository.DeleteAsync(created.Id);

        // Assert
        result.Should().BeTrue();
        var deletedUser = await _repository.GetByIdAsync(created.Id);
        deletedUser.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

