// User DTOs
namespace CA2_SOA.DTOs;

public record UserResponseDto(
    int Id,
    string Username,
    string FullName,
    string Email,
    string Role,
    DateTime CreatedAt,
    bool IsActive
);

public record CreateUserDto(
    string Username,
    string Password,
    string FullName,
    string Email,
    string Role
);

public record UpdateUserDto(
    string? FullName,
    string? Email,
    string? Role,
    bool? IsActive
);

public record LoginDto(
    string Username,
    string Password
);

public record LoginResponseDto(
    string Token,
    UserResponseDto User
);


