/// <summary>
/// Generic repository interface for common CRUD operations
/// Follows Repository Pattern for separation of concerns
/// </summary>
namespace CA2_SOA.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> CreateAsync(T entity);
    Task<T?> UpdateAsync(int id, T entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}


