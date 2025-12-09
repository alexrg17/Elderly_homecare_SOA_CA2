using Microsoft.EntityFrameworkCore;
using CA2_SOA.Data;
using CA2_SOA.Models;
using CA2_SOA.Interfaces;

namespace CA2_SOA.Repositories;

public class ResidentRepository : IResidentRepository
{
    private readonly CareHomeDbContext _context;
    
    public ResidentRepository(CareHomeDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Resident>> GetAllAsync()
    {
        return await _context.Residents
            .Include(r => r.Room)
            .ToListAsync();
    }
    
    public async Task<Resident?> GetByIdAsync(int id)
    {
        return await _context.Residents
            .Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
    
    public async Task<Resident?> GetResidentWithRoomAsync(int id)
    {
        return await _context.Residents
            .Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
    
    public async Task<IEnumerable<Resident>> GetActiveResidentsAsync()
    {
        return await _context.Residents
            .Include(r => r.Room)
            .Where(r => r.IsActive)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Resident>> GetResidentsByRoomAsync(int roomId)
    {
        return await _context.Residents
            .Include(r => r.Room)
            .Where(r => r.RoomId == roomId)
            .ToListAsync();
    }
    
    public async Task<Resident> CreateAsync(Resident entity)
    {
        _context.Residents.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public async Task<Resident?> UpdateAsync(int id, Resident entity)
    {
        var existing = await _context.Residents.FindAsync(id);
        if (existing == null) return null;
        
        _context.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return existing;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var resident = await _context.Residents.FindAsync(id);
        if (resident == null) return false;
        
        _context.Residents.Remove(resident);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Residents.AnyAsync(r => r.Id == id);
    }
}

