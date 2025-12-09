using CA2_SOA.Models;

namespace CA2_SOA.Interfaces;

public interface ISensorDataRepository : IRepository<SensorData>
{
    Task<IEnumerable<SensorData>> GetByRoomIdAsync(int roomId);
    Task<SensorData?> GetLatestByRoomIdAsync(int roomId);
    Task<IEnumerable<SensorData>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<SensorData>> GetRecentReadingsAsync(int count = 50);
}

