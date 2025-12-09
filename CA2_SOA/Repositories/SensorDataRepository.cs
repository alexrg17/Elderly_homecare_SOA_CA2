using Microsoft.EntityFrameworkCore;
using CA2_SOA.Data;
using CA2_SOA.Models;
using CA2_SOA.Interfaces;

namespace CA2_SOA.Repositories;

public class SensorDataRepository : ISensorDataRepository
{
    private readonly CareHomeDbContext _context;
    
    public SensorDataRepository(CareHomeDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<SensorData>> GetAllAsync()
    {
        return await _context.SensorReadings
            .Include(s => s.Room)
            .OrderByDescending(s => s.Timestamp)
            .ToListAsync();
    }
    
    public async Task<SensorData?> GetByIdAsync(int id)
    {
        return await _context.SensorReadings
            .Include(s => s.Room)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
    
    public async Task<IEnumerable<SensorData>> GetByRoomIdAsync(int roomId)
    {
        return await _context.SensorReadings
            .Include(s => s.Room)
            .Where(s => s.RoomId == roomId)
            .OrderByDescending(s => s.Timestamp)
            .ToListAsync();
    }
    
    public async Task<SensorData?> GetLatestByRoomIdAsync(int roomId)
    {
        return await _context.SensorReadings
            .Include(s => s.Room)
            .Where(s => s.RoomId == roomId)
            .OrderByDescending(s => s.Timestamp)
            .FirstOrDefaultAsync();
    }
    
    public async Task<IEnumerable<SensorData>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.SensorReadings
            .Include(s => s.Room)
            .Where(s => s.Timestamp >= startDate && s.Timestamp <= endDate)
            .OrderByDescending(s => s.Timestamp)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<SensorData>> GetRecentReadingsAsync(int count = 50)
    {
        return await _context.SensorReadings
            .Include(s => s.Room)
            .OrderByDescending(s => s.Timestamp)
            .Take(count)
            .ToListAsync();
    }
    
    public async Task<SensorData> CreateAsync(SensorData entity)
    {
        _context.SensorReadings.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public async Task<SensorData?> UpdateAsync(int id, SensorData entity)
    {
        var existing = await _context.SensorReadings.FindAsync(id);
        if (existing == null) return null;
        
        _context.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return existing;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var sensorData = await _context.SensorReadings.FindAsync(id);
        if (sensorData == null) return false;
        
        _context.SensorReadings.Remove(sensorData);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.SensorReadings.AnyAsync(s => s.Id == id);
    }
}

