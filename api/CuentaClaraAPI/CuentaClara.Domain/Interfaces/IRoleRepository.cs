using CuentaClara.Domain.Entities;

namespace CuentaClara.Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<ApplicationRole?> GetByIdAsync(string id);
        Task<ApplicationRole?> GetByRoleAsync(string role);
        Task<IEnumerable<ApplicationRole>> GetAllAsync();
        Task<bool> CreateAsync(ApplicationRole user);
        Task<bool> UpdateAsync(ApplicationRole user);
        Task<bool> DeleteAsync(string id);
    }
}
