using CuentaClara.Domain.Entities;
using CuentaClara.Domain.Interfaces;
using CuentaClara.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CuentaClara.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<AppUserRole> _roleManager;

        public RoleRepository(RoleManager<AppUserRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<bool> CreateAsync(ApplicationRole role)
        {
            var appRole = await _roleManager.FindByIdAsync(role.Id);
            if (appRole == null) return false;

            appRole.Name = role.Name;
            appRole.Description = role.Description;

            var result = await _roleManager.CreateAsync(appRole);
            return result.Succeeded;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return false;

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded;
        }

        public async Task<IEnumerable<ApplicationRole?>> GetAllAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles.Select((role)=> MapToDomainModel(role));
        }

        public async Task<ApplicationRole?> GetByIdAsync(string id)
        {
            var appUser = await _roleManager.FindByIdAsync(id);
            return appUser != null ? MapToDomainModel(appUser) : null;
        }

        private ApplicationRole? MapToDomainModel(AppUserRole appRole)
        {
            return new ApplicationRole
            {
                Id = appRole.Id,
                Name = appRole.Name,
                Description = appRole.Description,
            };
        }

        public async Task<ApplicationRole?> GetByRoleAsync(string role)
        {
            var appUser = await _roleManager.FindByNameAsync(role);
            return appUser != null ? MapToDomainModel(appUser) : null;
        }

        public async Task<bool> UpdateAsync(ApplicationRole role)
        {
            var appRole = await _roleManager.FindByIdAsync(role.Id);
            if (appRole == null) return false;

            appRole.Name = role.Name;
            appRole.Description = role.Description;

            var result = await _roleManager.UpdateAsync(appRole);
            return result.Succeeded;
        }
    }
}
