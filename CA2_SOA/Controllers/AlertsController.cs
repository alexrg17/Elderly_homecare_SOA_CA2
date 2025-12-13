using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CA2_SOA.DTOs;
using CA2_SOA.Interfaces;
using CA2_SOA.Models;

namespace CA2_SOA.Controllers;

/// <summary>
/// Controller for Alert CRUD operations
/// Manages alerts for environmental conditions in rooms
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlertsController : ControllerBase
{
    private readonly IAlertRepository _alertRepository;
    private readonly IUserRepository _userRepository;
    
    public AlertsController(IAlertRepository alertRepository, IUserRepository userRepository)
    {
        _alertRepository = alertRepository;
        _userRepository = userRepository;
    }
    
    /// <summary>
    /// Get all alerts
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlertDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AlertDto>>> GetAll()
    {
        var alerts = await _alertRepository.GetAllAsync();
        var alertDtos = new List<AlertDto>();
        
        foreach (var alert in alerts)
        {
            string? resolvedByUsername = null;
            if (alert.ResolvedByUserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(alert.ResolvedByUserId.Value);
                resolvedByUsername = user?.Username;
            }
            
            alertDtos.Add(new AlertDto(
                alert.Id,
                alert.RoomId,
                alert.Room.RoomNumber,
                alert.AlertType,
                alert.Severity,
                alert.Message,
                alert.CreatedAt,
                alert.IsResolved,
                alert.ResolvedAt,
                alert.ResolvedByUserId,
                resolvedByUsername,
                alert.ResolutionNotes
            ));
        }
        
        return Ok(alertDtos);
    }
    
    /// <summary>
    /// Get active (unresolved) alerts
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<AlertDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AlertDto>>> GetActive()
    {
        var alerts = await _alertRepository.GetActiveAlertsAsync();
        var alertDtos = alerts.Select(a => new AlertDto(
            a.Id,
            a.RoomId,
            a.Room.RoomNumber,
            a.AlertType,
            a.Severity,
            a.Message,
            a.CreatedAt,
            a.IsResolved,
            a.ResolvedAt,
            a.ResolvedByUserId,
            null,
            a.ResolutionNotes
        ));
        
        return Ok(alertDtos);
    }
    
    /// <summary>
    /// Get alert by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AlertDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertDto>> GetById(int id)
    {
        var alert = await _alertRepository.GetByIdAsync(id);
        
        if (alert == null)
            return NotFound(new { message = "Alert not found" });
        
        string? resolvedByUsername = null;
        if (alert.ResolvedByUserId.HasValue)
        {
            var user = await _userRepository.GetByIdAsync(alert.ResolvedByUserId.Value);
            resolvedByUsername = user?.Username;
        }
        
        var alertDto = new AlertDto(
            alert.Id,
            alert.RoomId,
            alert.Room.RoomNumber,
            alert.AlertType,
            alert.Severity,
            alert.Message,
            alert.CreatedAt,
            alert.IsResolved,
            alert.ResolvedAt,
            alert.ResolvedByUserId,
            resolvedByUsername,
            alert.ResolutionNotes
        );
        
        return Ok(alertDto);
    }
    
    /// <summary>
    /// Get alerts by room ID
    /// </summary>
    [HttpGet("room/{roomId}")]
    [ProducesResponseType(typeof(IEnumerable<AlertDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AlertDto>>> GetByRoom(int roomId)
    {
        var alerts = await _alertRepository.GetAlertsByRoomAsync(roomId);
        var alertDtos = alerts.Select(a => new AlertDto(
            a.Id,
            a.RoomId,
            a.Room.RoomNumber,
            a.AlertType,
            a.Severity,
            a.Message,
            a.CreatedAt,
            a.IsResolved,
            a.ResolvedAt,
            a.ResolvedByUserId,
            null,
            a.ResolutionNotes
        ));
        
        return Ok(alertDtos);
    }
    
    /// <summary>
    /// Get alerts by severity level
    /// </summary>
    [HttpGet("severity/{severity}")]
    [ProducesResponseType(typeof(IEnumerable<AlertDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AlertDto>>> GetBySeverity(string severity)
    {
        var alerts = await _alertRepository.GetAlertsBySeverityAsync(severity);
        var alertDtos = alerts.Select(a => new AlertDto(
            a.Id,
            a.RoomId,
            a.Room.RoomNumber,
            a.AlertType,
            a.Severity,
            a.Message,
            a.CreatedAt,
            a.IsResolved,
            a.ResolvedAt,
            a.ResolvedByUserId,
            null,
            a.ResolutionNotes
        ));
        
        return Ok(alertDtos);
    }
    
    /// <summary>
    /// Create a new alert (All authenticated users can create alerts)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Nurse,Caretaker")]
    [ProducesResponseType(typeof(AlertDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<AlertDto>> Create([FromBody] CreateAlertDto createDto)
    {
        var alert = new Alert
        {
            RoomId = createDto.RoomId,
            AlertType = createDto.AlertType,
            Severity = createDto.Severity,
            Message = createDto.Message,
            CreatedAt = DateTime.UtcNow,
            IsResolved = false
        };
        
        var createdEntity = await _alertRepository.CreateAsync(alert);
        
        var alertWithRoom = createdEntity.Room != null
            ? createdEntity
            : await _alertRepository.GetByIdAsync(createdEntity.Id);
        
        if (alertWithRoom == null || alertWithRoom.Room == null)
            return StatusCode(500, new { message = "Error retrieving created alert" });
        
        var alertDto = new AlertDto(
            alertWithRoom.Id,
            alertWithRoom.RoomId,
            alertWithRoom.Room.RoomNumber,
            alertWithRoom.AlertType,
            alertWithRoom.Severity,
            alertWithRoom.Message,
            alertWithRoom.CreatedAt,
            alertWithRoom.IsResolved,
            alertWithRoom.ResolvedAt,
            alertWithRoom.ResolvedByUserId,
            null,
            alertWithRoom.ResolutionNotes
        );
        
        return CreatedAtAction(nameof(GetById), new { id = alertWithRoom.Id }, alertDto);
    }
    
    /// <summary>
    /// Resolve an alert (All authenticated users can resolve alerts)
    /// </summary>
    [HttpPost("{id}/resolve")]
    [Authorize(Roles = "Admin,Nurse,Caretaker")]
    [ProducesResponseType(typeof(AlertDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertDto>> Resolve(int id, [FromBody] ResolveAlertDto resolveDto)
    {
        var resolved = await _alertRepository.ResolveAlertAsync(id, resolveDto.UserId, resolveDto.ResolutionNotes);
        
        if (resolved == null)
            return NotFound(new { message = "Alert not found" });
        
        // Reload to get Room navigation property
        var resolvedWithRoom = await _alertRepository.GetByIdAsync(resolved.Id);
        
        if (resolvedWithRoom == null)
            return StatusCode(500, new { message = "Error retrieving resolved alert" });
        
        string? resolvedByUsername = null;
        if (resolvedWithRoom.ResolvedByUserId.HasValue)
        {
            var user = await _userRepository.GetByIdAsync(resolvedWithRoom.ResolvedByUserId.Value);
            resolvedByUsername = user?.Username;
        }
        
        var alertDto = new AlertDto(
            resolvedWithRoom.Id,
            resolvedWithRoom.RoomId,
            resolvedWithRoom.Room.RoomNumber,
            resolvedWithRoom.AlertType,
            resolvedWithRoom.Severity,
            resolvedWithRoom.Message,
            resolvedWithRoom.CreatedAt,
            resolvedWithRoom.IsResolved,
            resolvedWithRoom.ResolvedAt,
            resolvedWithRoom.ResolvedByUserId,
            resolvedByUsername,
            resolvedWithRoom.ResolutionNotes
        );
        
        return Ok(alertDto);
    }
    
    /// <summary>
    /// Update alert (All authenticated users can update alerts)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Nurse,Caretaker")]
    [ProducesResponseType(typeof(AlertDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertDto>> Update(int id, [FromBody] UpdateAlertDto updateDto)
    {
        var existing = await _alertRepository.GetByIdAsync(id);
        
        if (existing == null)
            return NotFound(new { message = "Alert not found" });
        
        // Update only provided fields
        if (updateDto.Severity != null) existing.Severity = updateDto.Severity;
        if (updateDto.Message != null) existing.Message = updateDto.Message;
        if (updateDto.IsResolved.HasValue) existing.IsResolved = updateDto.IsResolved.Value;
        if (updateDto.ResolutionNotes != null) existing.ResolutionNotes = updateDto.ResolutionNotes;
        
        var updated = await _alertRepository.UpdateAsync(id, existing);
        
        if (updated == null)
            return NotFound(new { message = "Alert not found" });
        
        string? resolvedByUsername = null;
        if (updated.ResolvedByUserId.HasValue)
        {
            var user = await _userRepository.GetByIdAsync(updated.ResolvedByUserId.Value);
            resolvedByUsername = user?.Username;
        }
        
        var alertDto = new AlertDto(
            updated.Id,
            updated.RoomId,
            updated.Room.RoomNumber,
            updated.AlertType,
            updated.Severity,
            updated.Message,
            updated.CreatedAt,
            updated.IsResolved,
            updated.ResolvedAt,
            updated.ResolvedByUserId,
            resolvedByUsername,
            updated.ResolutionNotes
        );
        
        return Ok(alertDto);
    }
    
    /// <summary>
    /// Delete alert (All authenticated users can delete alerts)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Nurse,Caretaker")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _alertRepository.DeleteAsync(id);
        
        if (!success)
            return NotFound(new { message = "Alert not found" });
        
        return NoContent();
    }
}

