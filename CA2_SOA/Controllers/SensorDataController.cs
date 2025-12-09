using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CA2_SOA.DTOs;
using CA2_SOA.Interfaces;
using CA2_SOA.Models;

namespace CA2_SOA.Controllers;

/// <summary>
/// Controller for SensorData CRUD operations
/// Manages environmental sensor readings from rooms
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SensorDataController : ControllerBase
{
    private readonly ISensorDataRepository _sensorDataRepository;
    
    public SensorDataController(ISensorDataRepository sensorDataRepository)
    {
        _sensorDataRepository = sensorDataRepository;
    }
    
    /// <summary>
    /// Get all sensor readings
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SensorDataDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SensorDataDto>>> GetAll()
    {
        var sensorData = await _sensorDataRepository.GetAllAsync();
        var sensorDataDtos = sensorData.Select(s => new SensorDataDto(
            s.Id,
            s.RoomId,
            s.Room.RoomNumber,
            s.Temperature,
            s.Humidity,
            s.Timestamp,
            s.SensorType,
            s.Notes
        ));
        
        return Ok(sensorDataDtos);
    }
    
    /// <summary>
    /// Get recent sensor readings (last 50 by default)
    /// </summary>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(IEnumerable<SensorDataDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SensorDataDto>>> GetRecent([FromQuery] int count = 50)
    {
        var sensorData = await _sensorDataRepository.GetRecentReadingsAsync(count);
        var sensorDataDtos = sensorData.Select(s => new SensorDataDto(
            s.Id,
            s.RoomId,
            s.Room.RoomNumber,
            s.Temperature,
            s.Humidity,
            s.Timestamp,
            s.SensorType,
            s.Notes
        ));
        
        return Ok(sensorDataDtos);
    }
    
    /// <summary>
    /// Get sensor reading by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SensorDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SensorDataDto>> GetById(int id)
    {
        var sensorData = await _sensorDataRepository.GetByIdAsync(id);
        
        if (sensorData == null)
            return NotFound(new { message = "Sensor data not found" });
        
        var sensorDataDto = new SensorDataDto(
            sensorData.Id,
            sensorData.RoomId,
            sensorData.Room.RoomNumber,
            sensorData.Temperature,
            sensorData.Humidity,
            sensorData.Timestamp,
            sensorData.SensorType,
            sensorData.Notes
        );
        
        return Ok(sensorDataDto);
    }
    
    /// <summary>
    /// Get sensor readings by room ID
    /// </summary>
    [HttpGet("room/{roomId}")]
    [ProducesResponseType(typeof(IEnumerable<SensorDataDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SensorDataDto>>> GetByRoom(int roomId)
    {
        var sensorData = await _sensorDataRepository.GetByRoomIdAsync(roomId);
        var sensorDataDtos = sensorData.Select(s => new SensorDataDto(
            s.Id,
            s.RoomId,
            s.Room.RoomNumber,
            s.Temperature,
            s.Humidity,
            s.Timestamp,
            s.SensorType,
            s.Notes
        ));
        
        return Ok(sensorDataDtos);
    }
    
    /// <summary>
    /// Get latest sensor reading for a room
    /// </summary>
    [HttpGet("room/{roomId}/latest")]
    [ProducesResponseType(typeof(SensorDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SensorDataDto>> GetLatestByRoom(int roomId)
    {
        var sensorData = await _sensorDataRepository.GetLatestByRoomIdAsync(roomId);
        
        if (sensorData == null)
            return NotFound(new { message = "No sensor data found for this room" });
        
        var sensorDataDto = new SensorDataDto(
            sensorData.Id,
            sensorData.RoomId,
            sensorData.Room.RoomNumber,
            sensorData.Temperature,
            sensorData.Humidity,
            sensorData.Timestamp,
            sensorData.SensorType,
            sensorData.Notes
        );
        
        return Ok(sensorDataDto);
    }
    
    /// <summary>
    /// Get sensor readings by date range
    /// </summary>
    [HttpGet("daterange")]
    [ProducesResponseType(typeof(IEnumerable<SensorDataDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SensorDataDto>>> GetByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var sensorData = await _sensorDataRepository.GetByDateRangeAsync(startDate, endDate);
        var sensorDataDtos = sensorData.Select(s => new SensorDataDto(
            s.Id,
            s.RoomId,
            s.Room.RoomNumber,
            s.Temperature,
            s.Humidity,
            s.Timestamp,
            s.SensorType,
            s.Notes
        ));
        
        return Ok(sensorDataDtos);
    }
    
    /// <summary>
    /// Create a new sensor reading (typically from IoT devices)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SensorDataDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<SensorDataDto>> Create([FromBody] CreateSensorDataDto createDto)
    {
        var sensorData = new SensorData
        {
            RoomId = createDto.RoomId,
            Temperature = createDto.Temperature,
            Humidity = createDto.Humidity,
            SensorType = createDto.SensorType ?? "DHT22",
            Notes = createDto.Notes,
            Timestamp = DateTime.UtcNow
        };
        
        var created = await _sensorDataRepository.CreateAsync(sensorData);
        
        // Reload to get Room navigation property
        var createdWithRoom = await _sensorDataRepository.GetByIdAsync(created.Id);
        
        if (createdWithRoom == null)
            return StatusCode(500, new { message = "Error retrieving created sensor data" });
        
        var sensorDataDto = new SensorDataDto(
            createdWithRoom.Id,
            createdWithRoom.RoomId,
            createdWithRoom.Room.RoomNumber,
            createdWithRoom.Temperature,
            createdWithRoom.Humidity,
            createdWithRoom.Timestamp,
            createdWithRoom.SensorType,
            createdWithRoom.Notes
        );
        
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, sensorDataDto);
    }
    
    /// <summary>
    /// Update sensor reading
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SensorDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SensorDataDto>> Update(int id, [FromBody] UpdateSensorDataDto updateDto)
    {
        var existing = await _sensorDataRepository.GetByIdAsync(id);
        
        if (existing == null)
            return NotFound(new { message = "Sensor data not found" });
        
        // Update only provided fields
        if (updateDto.Temperature.HasValue) existing.Temperature = updateDto.Temperature.Value;
        if (updateDto.Humidity.HasValue) existing.Humidity = updateDto.Humidity.Value;
        if (updateDto.SensorType != null) existing.SensorType = updateDto.SensorType;
        if (updateDto.Notes != null) existing.Notes = updateDto.Notes;
        
        var updated = await _sensorDataRepository.UpdateAsync(id, existing);
        
        if (updated == null)
            return NotFound(new { message = "Sensor data not found" });
        
        var sensorDataDto = new SensorDataDto(
            updated.Id,
            updated.RoomId,
            updated.Room.RoomNumber,
            updated.Temperature,
            updated.Humidity,
            updated.Timestamp,
            updated.SensorType,
            updated.Notes
        );
        
        return Ok(sensorDataDto);
    }
    
    /// <summary>
    /// Delete sensor reading (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _sensorDataRepository.DeleteAsync(id);
        
        if (!success)
            return NotFound(new { message = "Sensor data not found" });
        
        return NoContent();
    }
}

