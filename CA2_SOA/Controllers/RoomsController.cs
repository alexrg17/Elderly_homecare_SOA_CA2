using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CA2_SOA.DTOs;
using CA2_SOA.Interfaces;
using CA2_SOA.Models;

namespace CA2_SOA.Controllers;

/// <summary>
/// Controller for Room CRUD operations
/// Manages rooms in the elderly care home
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly IRoomRepository _roomRepository;
    
    public RoomsController(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    
    /// <summary>
    /// Get all rooms
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll()
    {
        var rooms = await _roomRepository.GetAllAsync();
        var roomDtos = rooms.Select(r => new RoomDto(
            r.Id,
            r.RoomNumber,
            r.RoomName,
            r.Floor,
            r.Capacity,
            r.IsOccupied,
            r.Notes,
            r.CreatedAt,
            r.Residents.Count,
            r.SensorReadings.Count,
            r.Alerts.Count(a => !a.IsResolved)
        ));
        
        return Ok(roomDtos);
    }
    
    /// <summary>
    /// Get room by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomDto>> GetById(int id)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        
        if (room == null)
            return NotFound(new { message = "Room not found" });
        
        var roomDto = new RoomDto(
            room.Id,
            room.RoomNumber,
            room.RoomName,
            room.Floor,
            room.Capacity,
            room.IsOccupied,
            room.Notes,
            room.CreatedAt,
            room.Residents.Count,
            room.SensorReadings.Count,
            room.Alerts.Count(a => !a.IsResolved)
        );
        
        return Ok(roomDto);
    }
    
    /// <summary>
    /// Get room details with residents, latest sensor data, and active alerts
    /// </summary>
    [HttpGet("{id}/details")]
    [ProducesResponseType(typeof(RoomDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomDetailDto>> GetDetails(int id)
    {
        var room = await _roomRepository.GetRoomWithDetailsAsync(id);
        
        if (room == null)
            return NotFound(new { message = "Room not found" });
        
        var residents = room.Residents.Select(r => new ResidentDto(
            r.Id,
            r.FirstName,
            r.LastName,
            r.DateOfBirth,
            DateTime.Now.Year - r.DateOfBirth.Year,
            r.MedicalConditions,
            r.EmergencyContact,
            r.EmergencyPhone,
            r.AdmissionDate,
            r.IsActive,
            r.RoomId,
            r.Room?.RoomNumber
        )).ToList();
        
        var latestSensor = room.SensorReadings.OrderByDescending(s => s.Timestamp).FirstOrDefault();
        SensorDataDto? latestSensorDto = latestSensor != null ? new SensorDataDto(
            latestSensor.Id,
            latestSensor.RoomId,
            latestSensor.Room.RoomNumber,
            latestSensor.Temperature,
            latestSensor.Humidity,
            latestSensor.Timestamp,
            latestSensor.SensorType,
            latestSensor.Notes
        ) : null;
        
        var activeAlerts = room.Alerts.Where(a => !a.IsResolved).Select(a => new AlertDto(
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
        )).ToList();
        
        var roomDetailDto = new RoomDetailDto(
            room.Id,
            room.RoomNumber,
            room.RoomName,
            room.Floor,
            room.Capacity,
            room.IsOccupied,
            room.Notes,
            room.CreatedAt,
            residents,
            latestSensorDto,
            activeAlerts
        );
        
        return Ok(roomDetailDto);
    }
    
    /// <summary>
    /// Get occupied rooms
    /// </summary>
    [HttpGet("occupied")]
    [ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetOccupied()
    {
        var rooms = await _roomRepository.GetOccupiedRoomsAsync();
        var roomDtos = rooms.Select(r => new RoomDto(
            r.Id,
            r.RoomNumber,
            r.RoomName,
            r.Floor,
            r.Capacity,
            r.IsOccupied,
            r.Notes,
            r.CreatedAt,
            r.Residents.Count,
            r.SensorReadings.Count,
            r.Alerts.Count(a => !a.IsResolved)
        ));
        
        return Ok(roomDtos);
    }
    
    /// <summary>
    /// Get available rooms
    /// </summary>
    [HttpGet("available")]
    [ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAvailable()
    {
        var rooms = await _roomRepository.GetAvailableRoomsAsync();
        var roomDtos = rooms.Select(r => new RoomDto(
            r.Id,
            r.RoomNumber,
            r.RoomName,
            r.Floor,
            r.Capacity,
            r.IsOccupied,
            r.Notes,
            r.CreatedAt,
            r.Residents.Count,
            r.SensorReadings.Count,
            r.Alerts.Count(a => !a.IsResolved)
        ));
        
        return Ok(roomDtos);
    }
    
    /// <summary>
    /// Create a new room (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto createDto)
    {
        // Check if room number already exists
        if (await _roomRepository.RoomNumberExistsAsync(createDto.RoomNumber))
            return BadRequest(new { message = "Room number already exists" });
        
        var room = new Room
        {
            RoomNumber = createDto.RoomNumber,
            RoomName = createDto.RoomName,
            Floor = createDto.Floor,
            Capacity = createDto.Capacity,
            Notes = createDto.Notes,
            CreatedAt = DateTime.UtcNow
        };
        
        var created = await _roomRepository.CreateAsync(room);
        
        var roomDto = new RoomDto(
            created.Id,
            created.RoomNumber,
            created.RoomName,
            created.Floor,
            created.Capacity,
            created.IsOccupied,
            created.Notes,
            created.CreatedAt,
            0, 0, 0
        );
        
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, roomDto);
    }
    
    /// <summary>
    /// Update room (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoomDto>> Update(int id, [FromBody] UpdateRoomDto updateDto)
    {
        var existing = await _roomRepository.GetByIdAsync(id);
        
        if (existing == null)
            return NotFound(new { message = "Room not found" });
        
        // Update only provided fields
        if (updateDto.RoomNumber != null) existing.RoomNumber = updateDto.RoomNumber;
        if (updateDto.RoomName != null) existing.RoomName = updateDto.RoomName;
        if (updateDto.Floor != null) existing.Floor = updateDto.Floor;
        if (updateDto.Capacity.HasValue) existing.Capacity = updateDto.Capacity.Value;
        if (updateDto.IsOccupied.HasValue) existing.IsOccupied = updateDto.IsOccupied.Value;
        if (updateDto.Notes != null) existing.Notes = updateDto.Notes;
        
        var updated = await _roomRepository.UpdateAsync(id, existing);
        
        if (updated == null)
            return NotFound(new { message = "Room not found" });
        
        var roomDto = new RoomDto(
            updated.Id,
            updated.RoomNumber,
            updated.RoomName,
            updated.Floor,
            updated.Capacity,
            updated.IsOccupied,
            updated.Notes,
            updated.CreatedAt,
            updated.Residents.Count,
            updated.SensorReadings.Count,
            updated.Alerts.Count(a => !a.IsResolved)
        );
        
        return Ok(roomDto);
    }
    
    /// <summary>
    /// Delete room (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _roomRepository.DeleteAsync(id);
        
        if (!success)
            return NotFound(new { message = "Room not found" });
        
        return NoContent();
    }
}

