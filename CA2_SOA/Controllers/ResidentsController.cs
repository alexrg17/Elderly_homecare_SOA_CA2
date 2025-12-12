
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CA2_SOA.DTOs;
using CA2_SOA.Interfaces;
using CA2_SOA.Models;

namespace CA2_SOA.Controllers;

/// <summary>
/// Controller for Resident CRUD operations
/// Manages elderly residents in the care home
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResidentsController : ControllerBase
{
    private readonly IResidentRepository _residentRepository;
    
    public ResidentsController(IResidentRepository residentRepository)
    {
        _residentRepository = residentRepository;
    }
    
    /// <summary>
    /// Get all residents
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ResidentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ResidentDto>>> GetAll()
    {
        var residents = await _residentRepository.GetAllAsync();
        var residentDtos = residents.Select(r => new ResidentDto(
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
        ));
        
        return Ok(residentDtos);
    }
    
    /// <summary>
    /// Get active residents only
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<ResidentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ResidentDto>>> GetActive()
    {
        var residents = await _residentRepository.GetActiveResidentsAsync();
        var residentDtos = residents.Select(r => new ResidentDto(
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
        ));
        
        return Ok(residentDtos);
    }
    
    /// <summary>
    /// Get resident by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResidentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResidentDto>> GetById(int id)
    {
        var resident = await _residentRepository.GetByIdAsync(id);
        
        if (resident == null)
            return NotFound(new { message = "Resident not found" });
        
        var residentDto = new ResidentDto(
            resident.Id,
            resident.FirstName,
            resident.LastName,
            resident.DateOfBirth,
            DateTime.Now.Year - resident.DateOfBirth.Year,
            resident.MedicalConditions,
            resident.EmergencyContact,
            resident.EmergencyPhone,
            resident.AdmissionDate,
            resident.IsActive,
            resident.RoomId,
            resident.Room?.RoomNumber
        );
        
        return Ok(residentDto);
    }
    
    /// <summary>
    /// Get residents by room ID
    /// </summary>
    [HttpGet("room/{roomId}")]
    [ProducesResponseType(typeof(IEnumerable<ResidentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ResidentDto>>> GetByRoom(int roomId)
    {
        var residents = await _residentRepository.GetResidentsByRoomAsync(roomId);
        var residentDtos = residents.Select(r => new ResidentDto(
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
        ));
        
        return Ok(residentDtos);
    }
    
    /// <summary>
    /// Create a new resident (Admin and Nurse only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Nurse")]
    [ProducesResponseType(typeof(ResidentDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ResidentDto>> Create([FromBody] CreateResidentDto createDto)
    {
        var resident = new Resident
        {
            FirstName = createDto.FirstName,
            LastName = createDto.LastName,
            DateOfBirth = createDto.DateOfBirth,
            MedicalConditions = createDto.MedicalConditions,
            EmergencyContact = createDto.EmergencyContact,
            EmergencyPhone = createDto.EmergencyPhone,
            RoomId = createDto.RoomId,
            AdmissionDate = DateTime.UtcNow,
            IsActive = true
        };
        
        var created = await _residentRepository.CreateAsync(resident);
        
        var residentDto = new ResidentDto(
            created.Id,
            created.FirstName,
            created.LastName,
            created.DateOfBirth,
            DateTime.Now.Year - created.DateOfBirth.Year,
            created.MedicalConditions,
            created.EmergencyContact,
            created.EmergencyPhone,
            created.AdmissionDate,
            created.IsActive,
            created.RoomId,
            null
        );
        
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, residentDto);
    }
    
    /// <summary>
    /// Update resident (Admin and Nurse only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Nurse")]
    [ProducesResponseType(typeof(ResidentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResidentDto>> Update(int id, [FromBody] UpdateResidentDto updateDto)
    {
        var existing = await _residentRepository.GetByIdAsync(id);
        
        if (existing == null)
            return NotFound(new { message = "Resident not found" });
        
        // Update only provided fields
        if (updateDto.FirstName != null) existing.FirstName = updateDto.FirstName;
        if (updateDto.LastName != null) existing.LastName = updateDto.LastName;
        if (updateDto.DateOfBirth.HasValue) existing.DateOfBirth = updateDto.DateOfBirth.Value;
        if (updateDto.MedicalConditions != null) existing.MedicalConditions = updateDto.MedicalConditions;
        if (updateDto.EmergencyContact != null) existing.EmergencyContact = updateDto.EmergencyContact;
        if (updateDto.EmergencyPhone != null) existing.EmergencyPhone = updateDto.EmergencyPhone;
        if (updateDto.IsActive.HasValue) existing.IsActive = updateDto.IsActive.Value;
        if (updateDto.RoomId.HasValue) existing.RoomId = updateDto.RoomId.Value;
        
        var updated = await _residentRepository.UpdateAsync(id, existing);
        
        if (updated == null)
            return NotFound(new { message = "Resident not found" });
        
        var residentDto = new ResidentDto(
            updated.Id,
            updated.FirstName,
            updated.LastName,
            updated.DateOfBirth,
            DateTime.Now.Year - updated.DateOfBirth.Year,
            updated.MedicalConditions,
            updated.EmergencyContact,
            updated.EmergencyPhone,
            updated.AdmissionDate,
            updated.IsActive,
            updated.RoomId,
            updated.Room?.RoomNumber
        );
        
        return Ok(residentDto);
    }
    
    /// <summary>
    /// Delete resident (Admin and Nurse only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Nurse")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _residentRepository.DeleteAsync(id);
        
        if (!success)
            return NotFound(new { message = "Resident not found" });
        
        return NoContent();
    }
}
