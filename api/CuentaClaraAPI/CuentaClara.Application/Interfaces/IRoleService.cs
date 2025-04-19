using CuentaClara.Application.DTOs;
using CuentaClara.Application.DTOs.Role;

namespace CuentaClara.Application.Interfaces
{
    public interface IRoleService
    {
        Task<(bool Success, RoleDto? Role, string? ErrorMessage)> GetByIdAsync(string id);
        Task<(bool Success, RoleDto? Role, string? ErrorMessage)> GetByNameAsync(string name);
        Task<(bool Success, IEnumerable<RoleDto> Roles, string? ErrorMessage)> GetAllAsync();
        Task<(bool Success, string? RoleId, string? ErrorMessage)> CreateAsync(RoleDto roleDto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(string id, RoleDto userDto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(string id);
    }
}
