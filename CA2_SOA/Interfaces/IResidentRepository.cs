using CA2_SOA.Models;

namespace CA2_SOA.Interfaces;

public interface IResidentRepository : IRepository<Resident>
{
    Task<IEnumerable<Resident>> GetActiveResidentsAsync();
    Task<IEnumerable<Resident>> GetResidentsByRoomAsync(int roomId);
    Task<Resident?> GetResidentWithRoomAsync(int id);
}

