using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CA2_SOA.DTOs;
using CA2_SOA.Interfaces;
using CA2_SOA.Models;

namespace CA2_SOA.Controllers;

/// <summary>
/// Controller for User CRUD operations
/// Requires authentication for all endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    
    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _userRepository.GetAllAsync();
        var userDtos = users.Select(u => new UserDto(
            u.Id,
            u.Username,
            u.FullName,
            u.Email,
            u.Role,
            u.CreatedAt,
            u.IsActive
        ));
        
        return Ok(userDtos);
    }
    
    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        
        if (user == null)
            return NotFound(new { message = "User not found" });
        
        var userDto = new UserDto(
            user.Id,
            user.Username,
            user.FullName,
            user.Email,
            user.Role,
            user.CreatedAt,
            user.IsActive
        );
        
        return Ok(userDto);
    }
    
    /// <summary>
    /// Update user (Admin or own profile)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> Update(int id, [FromBody] UpdateUserDto updateDto)
    {
        var existing = await _userRepository.GetByIdAsync(id);
        
        if (existing == null)
            return NotFound(new { message = "User not found" });
        
        // Update only provided fields
        if (updateDto.FullName != null) existing.FullName = updateDto.FullName;
        if (updateDto.Email != null) existing.Email = updateDto.Email;
        if (updateDto.Role != null) existing.Role = updateDto.Role;
        if (updateDto.IsActive.HasValue) existing.IsActive = updateDto.IsActive.Value;
        
        var updated = await _userRepository.UpdateAsync(id, existing);
        
        if (updated == null)
            return NotFound(new { message = "User not found" });
        
        var userDto = new UserDto(
            updated.Id,
            updated.Username,
            updated.FullName,
            updated.Email,
            updated.Role,
            updated.CreatedAt,
            updated.IsActive
        );
        
        return Ok(userDto);
    }
    
    /// <summary>
    /// Delete user (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _userRepository.DeleteAsync(id);
        
        if (!success)
            return NotFound(new { message = "User not found" });
        
        return NoContent();
    }
}

