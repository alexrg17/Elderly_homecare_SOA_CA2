namespace CA2_SOA.DTOs;

// Resident DTOs
public record ResidentDto(
    int Id,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    int Age,
    string? MedicalConditions,
    string? EmergencyContact,
    string? EmergencyPhone,
    DateTime AdmissionDate,
    bool IsActive,
    int? RoomId,
    string? RoomNumber
);

public record CreateResidentDto(
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string? MedicalConditions,
    string? EmergencyContact,
    string? EmergencyPhone,
    int? RoomId
);

public record UpdateResidentDto(
    string? FirstName,
    string? LastName,
    DateTime? DateOfBirth,
    string? MedicalConditions,
    string? EmergencyContact,
    string? EmergencyPhone,
    bool? IsActive,
    int? RoomId
);

