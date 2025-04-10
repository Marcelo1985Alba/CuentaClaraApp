using CuentaClara.Application.DTOs;

namespace CuentaClara.Application.Interfaces
{
    public interface IUserService
    {
        Task<(bool Success, UserDto?, string? ErrorMessage)> GetByIdAsync(string id);
        Task<(bool Success, UserDto?, string? ErrorMessage)> GetByEmailAsync(string email);
        Task<(bool Success, IEnumerable<UserDto>, string? ErrorMessage)> GetAllAsync();
        Task<(bool Success, string UserId, string? ErrorMessage)> CreateAsync(CreateUserDto userDto);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(string id, UpdateUserDto userDto);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(string id);
        Task<((bool Success, UserDto? User, string? ErrorMessage) result, string? Token)> LoginAsync(LoginDto loginDto);
    }
}
