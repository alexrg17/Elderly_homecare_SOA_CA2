namespace CA2_SOA.DTOs;

// Room DTOs
public record RoomDto(
    int Id,
    string RoomNumber,
    string? RoomName,
    string Floor,
    int Capacity,
    bool IsOccupied,
    string? Notes,
    DateTime CreatedAt,
    int ResidentCount,
    int SensorReadingCount,
    int ActiveAlertCount
);

public record CreateRoomDto(
    string RoomNumber,
    string? RoomName,
    string Floor,
    int Capacity,
    string? Notes
);

public record UpdateRoomDto(
    string? RoomNumber,
    string? RoomName,
    string? Floor,
    int? Capacity,
    bool? IsOccupied,
    string? Notes
);

public record RoomDetailDto(
    int Id,
    string RoomNumber,
    string? RoomName,
    string Floor,
    int Capacity,
    bool IsOccupied,
    string? Notes,
    DateTime CreatedAt,
    List<ResidentDto> Residents,
    SensorDataDto? LatestSensorReading,
    List<AlertDto> ActiveAlerts
);

