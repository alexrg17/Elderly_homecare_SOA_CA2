using Microsoft.EntityFrameworkCore;
}
    }
        return await _context.Users.AnyAsync(u => u.Email == email);
    {
    public async Task<bool> EmailExistsAsync(string email)
    
    }
        return await _context.Users.AnyAsync(u => u.Username == username);
    {
    public async Task<bool> UsernameExistsAsync(string username)
    
    }
        return await _context.Users.AnyAsync(u => u.Id == id);
    {
    public async Task<bool> ExistsAsync(int id)
    
    }
        return true;
        await _context.SaveChangesAsync();
        _context.Users.Remove(user);
        
        if (user == null) return false;
        var user = await _context.Users.FindAsync(id);
    {
    public async Task<bool> DeleteAsync(int id)
    
    }
        return existing;
        await _context.SaveChangesAsync();
        _context.Entry(existing).CurrentValues.SetValues(entity);
        
        if (existing == null) return null;
        var existing = await _context.Users.FindAsync(id);
    {
    public async Task<User?> UpdateAsync(int id, User entity)
    
    }
        return entity;
        await _context.SaveChangesAsync();
        _context.Users.Add(entity);
    {
    public async Task<User> CreateAsync(User entity)
    
    }
            .FirstOrDefaultAsync(u => u.Email == email);
        return await _context.Users
    {
    public async Task<User?> GetByEmailAsync(string email)
    
    }
            .FirstOrDefaultAsync(u => u.Username == username);
        return await _context.Users
    {
    public async Task<User?> GetByUsernameAsync(string username)
    
    }
        return await _context.Users.FindAsync(id);
    {
    public async Task<User?> GetByIdAsync(int id)
    
    }
        return await _context.Users.ToListAsync();
    {
    public async Task<IEnumerable<User>> GetAllAsync()
    
    }
        _context = context;
    {
    public UserRepository(CareHomeDbContext context)
    
    private readonly CareHomeDbContext _context;
{
public class UserRepository : IUserRepository

namespace CA2_SOA.Repositories;

using CA2_SOA.Interfaces;
using CA2_SOA.Models;
using CA2_SOA.Data;

