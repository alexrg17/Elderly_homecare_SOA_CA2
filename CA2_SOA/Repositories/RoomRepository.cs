using Microsoft.EntityFrameworkCore;
using CA2_SOA.Data;
using CA2_SOA.Models;
using CA2_SOA.Interfaces;

namespace CA2_SOA.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly CareHomeDbContext _context;
    
    public RoomRepository(CareHomeDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        return await _context.Rooms
            .Include(r => r.Residents)
            .ToListAsync();
    }
    
    public async Task<Room?> GetByIdAsync(int id)
    {
        return await _context.Rooms
            .Include(r => r.Residents)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
    
    public async Task<Room?> GetRoomWithDetailsAsync(int id)
    {
        return await _context.Rooms
            .Include(r => r.Residents)
            .Include(r => r.SensorReadings.OrderByDescending(s => s.Timestamp).Take(1))
            .Include(r => r.Alerts.Where(a => !a.IsResolved))
            .FirstOrDefaultAsync(r => r.Id == id);
    }
    
    public async Task<Room?> GetByRoomNumberAsync(string roomNumber)
    {
        return await _context.Rooms
            .FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);
    }
    
    public async Task<Room> CreateAsync(Room entity)
    {
        _context.Rooms.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public async Task<Room?> UpdateAsync(int id, Room entity)
    {
        var existing = await _context.Rooms.FindAsync(id);
        if (existing == null) return null;
        
        _context.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return existing;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return false;
        
        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Rooms.AnyAsync(r => r.Id == id);
    }
    
    public async Task<bool> RoomNumberExistsAsync(string roomNumber)
    {
        return await _context.Rooms.AnyAsync(r => r.RoomNumber == roomNumber);
    }
    
    public async Task<IEnumerable<Room>> GetOccupiedRoomsAsync()
    {
        return await _context.Rooms
            .Include(r => r.Residents)
            .Where(r => r.IsOccupied)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync()
    {
        return await _context.Rooms
            .Include(r => r.Residents)
            .Where(r => !r.IsOccupied)
            .ToListAsync();
    }
}

