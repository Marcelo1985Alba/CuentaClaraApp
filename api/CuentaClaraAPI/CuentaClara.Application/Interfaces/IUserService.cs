using CuentaClara.Application.DTOs;

namespace CuentaClara.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetByIdAsync(string id);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<(bool Success, string UserId)> CreateAsync(CreateUserDto userDto);
        Task<bool> UpdateAsync(string id, UpdateUserDto userDto);
        Task<bool> DeleteAsync(string id);
        Task<(bool Success, string? Token)> LoginAsync(LoginDto loginDto);
    }
}
