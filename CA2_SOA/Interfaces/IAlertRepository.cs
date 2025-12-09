using CA2_SOA.Models;

namespace CA2_SOA.Interfaces;

public interface IAlertRepository : IRepository<Alert>
{
    Task<IEnumerable<Alert>> GetActiveAlertsAsync();
    Task<IEnumerable<Alert>> GetAlertsByRoomAsync(int roomId);
    Task<IEnumerable<Alert>> GetAlertsBySeverityAsync(string severity);
    Task<Alert?> ResolveAlertAsync(int alertId, int userId, string? notes);
}

