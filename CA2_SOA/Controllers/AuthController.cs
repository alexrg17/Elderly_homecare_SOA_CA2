using Microsoft.AspNetCore.Mvc;
}
    }
        return CreatedAtAction(nameof(Register), new { id = user.Id }, userDto);
        
        );
            user.IsActive
            user.CreatedAt,
            user.Role,
            user.Email,
            user.FullName,
            user.Username,
            user.Id,
        var userDto = new UserDto(
        
            return BadRequest(new { message = "Username or email already exists" });
        if (user == null)
        
        var user = await _authService.RegisterAsync(createUserDto);
    {
    public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserDto createUserDto)
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [HttpPost("register")]
    /// </summary>
    /// Register a new user
    /// <summary>
    
    }
        return Ok(result);
        
            return Unauthorized(new { message = "Invalid username or password" });
        if (result == null)
        
        var result = await _authService.LoginAsync(loginDto);
    {
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [HttpPost("login")]
    /// </summary>
    /// Login with username and password
    /// <summary>
    
    }
        _authService = authService;
    {
    public AuthController(AuthService authService)
    
    private readonly AuthService _authService;
{
public class AuthController : ControllerBase
[Route("api/[controller]")]
[ApiController]
/// </summary>
/// Controller for authentication operations (login, register)
/// <summary>

namespace CA2_SOA.Controllers;

using CA2_SOA.Services;
using CA2_SOA.DTOs;

