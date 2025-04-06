using CuentaClara.Domain.Entities;

namespace CuentaClara.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<ApplicationUser?> GetByUsernameAsync(string username);
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<bool> CreateAsync(ApplicationUser user, string password);
        Task<bool> UpdateAsync(ApplicationUser user);
        Task<bool> DeleteAsync(string id);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    }
}
