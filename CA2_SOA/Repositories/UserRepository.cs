
using Microsoft.EntityFrameworkCore;
using CA2_SOA.Data;
using CA2_SOA.Models;
using CA2_SOA.Interfaces;

namespace CA2_SOA.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CareHomeDbContext _context;
    
    public UserRepository(CareHomeDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }
    
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }
    
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<User> CreateAsync(User entity)
    {
        _context.Users.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    
    public async Task<User?> UpdateAsync(int id, User entity)
    {
        var existing = await _context.Users.FindAsync(id);
        if (existing == null) return null;
        
        // Update properties
        existing.Username = entity.Username;
        existing.FullName = entity.FullName;
        existing.Email = entity.Email;
        existing.Role = entity.Role;
        existing.IsActive = entity.IsActive;
        if (!string.IsNullOrEmpty(entity.PasswordHash))
        {
            existing.PasswordHash = entity.PasswordHash;
        }
        
        _context.Users.Update(existing);
        await _context.SaveChangesAsync();
        return existing;
    }
    
    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;
        
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }
    
    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }
    
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
}
