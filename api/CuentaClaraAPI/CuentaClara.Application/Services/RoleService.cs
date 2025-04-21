using CuentaClara.Application.DTOs;
using CuentaClara.Application.DTOs.Role;
using CuentaClara.Application.Interfaces;
using CuentaClara.Domain.Entities;
using CuentaClara.Domain.Interfaces;

namespace CuentaClara.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<(bool Success, string? RoleId, string? ErrorMessage)> CreateAsync(RoleDto roleDto)
        {
            var role = new ApplicationRole
            {
                Name = roleDto.Name,
                Description = roleDto.Description,
            };

            var result = await _roleRepository.CreateAsync(role);
            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", "Error al crear rol");
                return (false, null, errorMessage);
            }
            return (result.Succeeded, result.applicationRole?.Id, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(string id)
        {
            var result = await _roleRepository.DeleteAsync(id);
            if (!result)
                return (false, "Error al eliminar rol");

            return (true, null);
        }

        public async Task<(bool Success, IEnumerable<RoleDto> Roles, string? ErrorMessage)> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            if (roles == null || !roles.Any())
                return (false, Enumerable.Empty<RoleDto>(), "No se encontraron roles");

            var roleDtos = roles.Select(role => MapToDto(role));
            return (true, roleDtos, null);
        }

        public async Task<(bool Success, RoleDto? Role, string? ErrorMessage)> GetByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                return (false, null, "El rol de usuario es requerido");

            var role = await _roleRepository.GetByRoleAsync(name);

            if (role == null)
                return (false, null, "Rol no encontrado");

            return (true, MapToDto(role), null);
        }

        public async Task<(bool Success, RoleDto? Role, string? ErrorMessage)> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return (false, null, "El id de rol es requerido");

            var role = await _roleRepository.GetByIdAsync(id);

            if (role == null)
                return (false, null, "Rol no encontrado");

            return (true, MapToDto(role), null);
        }

        private static RoleDto? MapToDto(ApplicationRole role)
        {
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
            };
        }

        public Task<(bool Success, string? ErrorMessage)> UpdateAsync(string id, RoleDto userDto)
        {
            throw new NotImplementedException();
        }

    }
}
