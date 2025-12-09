using Microsoft.EntityFrameworkCore;
using CA2_SOA.Data;
using CA2_SOA.Models;
using CA2_SOA.Interfaces;

namespace CA2_SOA.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly CareHomeDbContext _context;
    
    public AlertRepository(CareHomeDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Alert>> GetAllAsync()
    {
        return await _context.Alerts
            .Include(a => a.Room)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<Alert?> GetByIdAsync(int id)
    {
        return await _context.Alerts
            .Include(a => a.Room)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
    
    public async Task<IEnumerable<Alert>> GetActiveAlertsAsync()
    {
        return await _context.Alerts
            .Include(a => a.Room)
            .Where(a => !a.IsResolved)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Alert>> GetAlertsByRoomAsync(int roomId)
    {
        return await _context.Alerts
            .Include(a => a.Room)
            .Where(a => a.RoomId == roomId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Alert>> GetAlertsBySeverityAsync(string severity)
    {
        return await _context.Alerts
            .Include(a => a.Room)
            .Where(a => a.Severity == severity)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<Alert?> ResolveAlertAsync(int alertId, int userId, string? notes)
    {
        var alert = await _context.Alerts.FindAsync(alertId);
        if (alert == null) return null;
        
        alert.IsResolved = true;
        alert.ResolvedAt = DateTime.UtcNow;
        alert.ResolvedByUserId = userId;
        alert.ResolutionNotes = notes;
        
        await _context.SaveChangesAsync();
        return alert;
    }
    
    public async Task<Alert> CreateAsync(Alert entity)
    {
        _context.Alerts.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public async Task<Alert?> UpdateAsync(int id, Alert entity)
    {
        var existing = await _context.Alerts.FindAsync(id);
        if (existing == null) return null;
        
        _context.Entry(existing).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return existing;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var alert = await _context.Alerts.FindAsync(id);
        if (alert == null) return false;
        
        _context.Alerts.Remove(alert);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Alerts.AnyAsync(a => a.Id == id);
    }
}

