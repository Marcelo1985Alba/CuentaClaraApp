using CuentaClara.Domain.Entities;
using CuentaClara.Domain.Interfaces;
using CuentaClara.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CuentaClara.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public UserRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            var appUser = await _userManager.FindByIdAsync(id);
            return appUser != null ? await MapToDomainModel(appUser) : null;
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            var appUser = await _userManager.FindByEmailAsync(email);
            return appUser != null ? await MapToDomainModel(appUser) : null;
        }

        public async Task<ApplicationUser?> GetByUsernameAsync(string username)
        {
            var appUser = await _userManager.FindByNameAsync(username);

            return appUser != null ? await MapToDomainModel(appUser) : null;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            var appUsers = await _userManager.Users.ToListAsync();
            // Con Task.WhenAll para procesar en paralelo:
            var tasks = appUsers.Select(user => MapToDomainModel(user));
            var results = await Task.WhenAll(tasks);
            return results;
        }

        public async Task<bool> CreateAsync(ApplicationUser user, string password)
        {
            var appUser = new AppUser
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ImageUrl = user.ImageUrl
            };

            var result = await _userManager.CreateAsync(appUser, password);

            if (result.Succeeded)
            {
                user.Id = appUser.Id;
                return true;
            }

            return false;
        }

        public async Task<bool> UpdateAsync(ApplicationUser user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id);
            if (appUser == null) return false;

            appUser.FirstName = user.FirstName;
            appUser.LastName = user.LastName;
            appUser.PhoneNumber = user.PhoneNumber;
            appUser.ImageUrl = user.ImageUrl;
            appUser.Email = user.Email;

            var result = await _userManager.UpdateAsync(appUser);

            //Actualizar roles si se especificaron

            if (user.Roles != null && user.Roles.Count > 0) 
            {
                var currentRoles = await _userManager.GetRolesAsync(appUser);
                var rolesToAdd = user.Roles.Except(currentRoles).ToList();
                var rolesToRemove = currentRoles.Except(user.Roles).ToList();
                if (rolesToAdd.Count > 0)
                {
                    var resultAddRoles = await _userManager.AddToRolesAsync(appUser, rolesToAdd);
                    if (!resultAddRoles.Succeeded)
                    {
                        var errorMessage = string.Join(", ", resultAddRoles.Errors.Select(e => e.Description));
                        return false;
                    }

                }
                if (rolesToRemove.Count > 0)
                {
                    var resultReomveRoles = await _userManager.RemoveFromRolesAsync(appUser, rolesToRemove);
                    if (!resultReomveRoles.Succeeded)
                    {
                        var errorMessage = string.Join(", ", resultReomveRoles.Errors.Select(e => e.Description));
                        return false;
                    }

                }
            }



            return result.Succeeded;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null) return false;

            var result = await _userManager.DeleteAsync(appUser);
            return result.Succeeded;
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id);
            if (appUser == null) return false;

            return await _userManager.CheckPasswordAsync(appUser, password);
        }

        private async Task<ApplicationUser> MapToDomainModel(AppUser appUser)
        {
            var roles = await _userManager.GetRolesAsync(appUser);
            return new ApplicationUser
            {
                Id = appUser.Id,
                UserName = appUser.UserName ?? string.Empty,
                Email = appUser.Email ?? string.Empty,
                PhoneNumber = appUser.PhoneNumber ?? string.Empty,
                EmailConfirmed = appUser.EmailConfirmed,
                PhoneNumberConfirmed = appUser.PhoneNumberConfirmed,
                TwoFactorEnabled = appUser.TwoFactorEnabled,
                LockoutEnabled = appUser.LockoutEnabled,
                AccessFailedCount = appUser.AccessFailedCount,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                ImageUrl = appUser.ImageUrl,
                Roles = roles
            };
        }
    }
}
