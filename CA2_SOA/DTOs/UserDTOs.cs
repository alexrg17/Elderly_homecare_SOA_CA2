namespace CA2_SOA.DTOs;
);
    UserDto User
    string Token,
public record LoginResponseDto(

);
    string Password
    string Username,
public record LoginDto(

);
    bool? IsActive
    string? Role,
    string? Email,
    string? FullName,
public record UpdateUserDto(

);
    string Role
    string Email,
    string FullName,
    string Password,
    string Username,
public record CreateUserDto(

);
    bool IsActive
    DateTime CreatedAt,
    string Role,
    string Email,
    string FullName,
    string Username,
    int Id,
public record UserDto(
// User DTOs


