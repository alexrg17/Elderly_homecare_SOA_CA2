namespace CA2_SOA.DTOs;

// SensorData DTOs
public record SensorDataDto(
    int Id,
    int RoomId,
    string RoomNumber,
    decimal Temperature,
    decimal Humidity,
    DateTime Timestamp,
    string SensorType,
    string? Notes
);

public record CreateSensorDataDto(
    int RoomId,
    decimal Temperature,
    decimal Humidity,
    string? SensorType,
    string? Notes
);

public record UpdateSensorDataDto(
    decimal? Temperature,
    decimal? Humidity,
    string? SensorType,
    string? Notes
);

