using CA2_SOA.Models;

namespace CA2_SOA.Interfaces;

public interface IRoomRepository : IRepository<Room>
{
    Task<Room?> GetRoomWithDetailsAsync(int id);
    Task<Room?> GetByRoomNumberAsync(string roomNumber);
    Task<bool> RoomNumberExistsAsync(string roomNumber);
    Task<IEnumerable<Room>> GetOccupiedRoomsAsync();
    Task<IEnumerable<Room>> GetAvailableRoomsAsync();
}

