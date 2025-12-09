using Microsoft.AspNetCore.Mvc;
using CA2_SOA.DTOs;
using CA2_SOA.Services;

namespace CA2_SOA.Controllers;

/// <summary>
/// Controller for authentication operations (login, register)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }
    
    /// <summary>
    /// Login with username and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        
        if (result == null)
            return Unauthorized(new { message = "Invalid username or password" });
        
        return Ok(result);
    }
    
    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponseDto>> Register([FromBody] CreateUserDto createUserDto)
    {
        var user = await _authService.RegisterAsync(createUserDto);
        
        if (user == null)
            return BadRequest(new { message = "Username or email already exists" });
        
        var userDto = new UserResponseDto(
            user.Id,
            user.Username,
            user.FullName,
            user.Email,
            user.Role,
            user.CreatedAt,
            user.IsActive
        );
        
        return CreatedAtAction(nameof(Register), new { id = user.Id }, userDto);
    }
}

