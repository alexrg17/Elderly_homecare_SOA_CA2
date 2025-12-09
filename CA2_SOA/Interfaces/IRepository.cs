namespace CA2_SOA.Interfaces;
}
    Task<bool> ExistsAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<T?> UpdateAsync(int id, T entity);
    Task<T> CreateAsync(T entity);
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
{
public interface IRepository<T> where T : class
/// </summary>
/// Follows Repository Pattern for separation of concerns
/// Generic repository interface for common CRUD operations
/// <summary>


