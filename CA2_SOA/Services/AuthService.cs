using CA2_SOA.Models;
using CA2_SOA.DTOs;
using CA2_SOA.Interfaces;

namespace CA2_SOA.Services;

/// <summary>
/// Service for handling user authentication
/// </summary>
public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtService _jwtService;
    
    public AuthService(IUserRepository userRepository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }
    
    public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
        
        if (user == null || !user.IsActive)
            return null;
        
        // Verify password using BCrypt
        // Source: BCrypt.Net documentation - https://github.com/BcryptNet/bcrypt.net
        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            return null;
        
        var token = _jwtService.GenerateToken(user);
        
        var userDto = new UserResponseDto(
            user.Id,
            user.Username,
            user.FullName,
            user.Email,
            user.Role,
            user.CreatedAt,
            user.IsActive
        );
        
        return new LoginResponseDto(token, userDto);
    }
    
    public async Task<User?> RegisterAsync(CreateUserDto createUserDto)
    {
        // Check if username already exists
        if (await _userRepository.UsernameExistsAsync(createUserDto.Username))
            return null;
        
        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(createUserDto.Email))
            return null;
        
        var user = new User
        {
            Username = createUserDto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
            FullName = createUserDto.FullName,
            Email = createUserDto.Email,
            Role = createUserDto.Role,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        
        return await _userRepository.CreateAsync(user);
    }
}

