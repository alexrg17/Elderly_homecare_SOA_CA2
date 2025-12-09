using Microsoft.AspNetCore.Authorization;
}
    }
        return NoContent();
        
            return NotFound(new { message = "Resident not found" });
        if (!success)
        
        var success = await _residentRepository.DeleteAsync(id);
    {
    public async Task<IActionResult> Delete(int id)
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    /// </summary>
    /// Delete resident (Admin only)
    /// <summary>
    
    }
        return Ok(residentDto);
        
        );
            updated.Room?.RoomNumber
            updated.RoomId,
            updated.IsActive,
            updated.AdmissionDate,
            updated.EmergencyPhone,
            updated.EmergencyContact,
            updated.MedicalConditions,
            DateTime.Now.Year - updated.DateOfBirth.Year,
            updated.DateOfBirth,
            updated.LastName,
            updated.FirstName,
            updated.Id,
        var residentDto = new ResidentDto(
        
            return NotFound(new { message = "Resident not found" });
        if (updated == null)
        
        var updated = await _residentRepository.UpdateAsync(id, existing);
        
        if (updateDto.RoomId.HasValue) existing.RoomId = updateDto.RoomId.Value;
        if (updateDto.IsActive.HasValue) existing.IsActive = updateDto.IsActive.Value;
        if (updateDto.EmergencyPhone != null) existing.EmergencyPhone = updateDto.EmergencyPhone;
        if (updateDto.EmergencyContact != null) existing.EmergencyContact = updateDto.EmergencyContact;
        if (updateDto.MedicalConditions != null) existing.MedicalConditions = updateDto.MedicalConditions;
        if (updateDto.DateOfBirth.HasValue) existing.DateOfBirth = updateDto.DateOfBirth.Value;
        if (updateDto.LastName != null) existing.LastName = updateDto.LastName;
        if (updateDto.FirstName != null) existing.FirstName = updateDto.FirstName;
        // Update only provided fields
        
            return NotFound(new { message = "Resident not found" });
        if (existing == null)
        
        var existing = await _residentRepository.GetByIdAsync(id);
    {
    public async Task<ActionResult<ResidentDto>> Update(int id, [FromBody] UpdateResidentDto updateDto)
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResidentDto), StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin,Caretaker")]
    [HttpPut("{id}")]
    /// </summary>
    /// Update resident
    /// <summary>
    
    }
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, residentDto);
        
        );
            null
            created.RoomId,
            created.IsActive,
            created.AdmissionDate,
            created.EmergencyPhone,
            created.EmergencyContact,
            created.MedicalConditions,
            DateTime.Now.Year - created.DateOfBirth.Year,
            created.DateOfBirth,
            created.LastName,
            created.FirstName,
            created.Id,
        var residentDto = new ResidentDto(
        
        var created = await _residentRepository.CreateAsync(resident);
        
        };
            IsActive = true
            AdmissionDate = DateTime.UtcNow,
            RoomId = createDto.RoomId,
            EmergencyPhone = createDto.EmergencyPhone,
            EmergencyContact = createDto.EmergencyContact,
            MedicalConditions = createDto.MedicalConditions,
            DateOfBirth = createDto.DateOfBirth,
            LastName = createDto.LastName,
            FirstName = createDto.FirstName,
        {
        var resident = new Resident
    {
    public async Task<ActionResult<ResidentDto>> Create([FromBody] CreateResidentDto createDto)
    [ProducesResponseType(typeof(ResidentDto), StatusCodes.Status201Created)]
    [Authorize(Roles = "Admin,Caretaker")]
    [HttpPost]
    /// </summary>
    /// Create a new resident
    /// <summary>
    
    }
        return Ok(residentDtos);
        
        ));
            r.Room?.RoomNumber
            r.RoomId,
            r.IsActive,
            r.AdmissionDate,
            r.EmergencyPhone,
            r.EmergencyContact,
            r.MedicalConditions,
            DateTime.Now.Year - r.DateOfBirth.Year,
            r.DateOfBirth,
            r.LastName,
            r.FirstName,
            r.Id,
        var residentDtos = residents.Select(r => new ResidentDto(
        var residents = await _residentRepository.GetResidentsByRoomAsync(roomId);
    {
    public async Task<ActionResult<IEnumerable<ResidentDto>>> GetByRoom(int roomId)
    [ProducesResponseType(typeof(IEnumerable<ResidentDto>), StatusCodes.Status200OK)]
    [HttpGet("room/{roomId}")]
    /// </summary>
    /// Get residents by room ID
    /// <summary>
    
    }
        return Ok(residentDto);
        
        );
            resident.Room?.RoomNumber
            resident.RoomId,
            resident.IsActive,
            resident.AdmissionDate,
            resident.EmergencyPhone,
            resident.EmergencyContact,
            resident.MedicalConditions,
            DateTime.Now.Year - resident.DateOfBirth.Year,
            resident.DateOfBirth,
            resident.LastName,
            resident.FirstName,
            resident.Id,
        var residentDto = new ResidentDto(
        
            return NotFound(new { message = "Resident not found" });
        if (resident == null)
        
        var resident = await _residentRepository.GetByIdAsync(id);
    {
    public async Task<ActionResult<ResidentDto>> GetById(int id)
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResidentDto), StatusCodes.Status200OK)]
    [HttpGet("{id}")]
    /// </summary>
    /// Get resident by ID
    /// <summary>
    
    }
        return Ok(residentDtos);
        
        ));
            r.Room?.RoomNumber
            r.RoomId,
            r.IsActive,
            r.AdmissionDate,
            r.EmergencyPhone,
            r.EmergencyContact,
            r.MedicalConditions,
            DateTime.Now.Year - r.DateOfBirth.Year,
            r.DateOfBirth,
            r.LastName,
            r.FirstName,
            r.Id,
        var residentDtos = residents.Select(r => new ResidentDto(
        var residents = await _residentRepository.GetActiveResidentsAsync();
    {
    public async Task<ActionResult<IEnumerable<ResidentDto>>> GetActive()
    [ProducesResponseType(typeof(IEnumerable<ResidentDto>), StatusCodes.Status200OK)]
    [HttpGet("active")]
    /// </summary>
    /// Get active residents only
    /// <summary>
    
    }
        return Ok(residentDtos);
        
        ));
            r.Room?.RoomNumber
            r.RoomId,
            r.IsActive,
            r.AdmissionDate,
            r.EmergencyPhone,
            r.EmergencyContact,
            r.MedicalConditions,
            DateTime.Now.Year - r.DateOfBirth.Year,
            r.DateOfBirth,
            r.LastName,
            r.FirstName,
            r.Id,
        var residentDtos = residents.Select(r => new ResidentDto(
        var residents = await _residentRepository.GetAllAsync();
    {
    public async Task<ActionResult<IEnumerable<ResidentDto>>> GetAll()
    [ProducesResponseType(typeof(IEnumerable<ResidentDto>), StatusCodes.Status200OK)]
    [HttpGet]
    /// </summary>
    /// Get all residents
    /// <summary>
    
    }
        _residentRepository = residentRepository;
    {
    public ResidentsController(IResidentRepository residentRepository)
    
    private readonly IResidentRepository _residentRepository;
{
public class ResidentsController : ControllerBase
[Authorize]
[Route("api/[controller]")]
[ApiController]
/// </summary>
/// Manages elderly residents in the care home
/// Controller for Resident CRUD operations
/// <summary>

namespace CA2_SOA.Controllers;

using CA2_SOA.Models;
using CA2_SOA.Interfaces;
using CA2_SOA.DTOs;
using Microsoft.AspNetCore.Mvc;

