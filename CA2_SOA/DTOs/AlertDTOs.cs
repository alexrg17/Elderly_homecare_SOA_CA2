namespace CA2_SOA.DTOs;

// Alert DTOs
public record AlertDto(
    int Id,
    int RoomId,
    string RoomNumber,
    string AlertType,
    string Severity,
    string Message,
    DateTime CreatedAt,
    bool IsResolved,
    DateTime? ResolvedAt,
    int? ResolvedByUserId,
    string? ResolvedByUsername,
    string? ResolutionNotes
);

public record CreateAlertDto(
    int RoomId,
    string AlertType,
    string Severity,
    string Message
);

public record UpdateAlertDto(
    string? Severity,
    string? Message,
    bool? IsResolved,
    string? ResolutionNotes
);

public record ResolveAlertDto(
    int UserId,
    string? ResolutionNotes
);

