using CuentaClara.Application.DTOs;
using CuentaClara.Application.Interfaces;
using CuentaClara.Domain.Entities;
using CuentaClara.Domain.Interfaces;

namespace CuentaClara.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtGenerator _jwtGenerator;

        public UserService(IUserRepository userRepository, IJwtGenerator jwtGenerator)
        {
            _userRepository = userRepository;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<UserDto?> GetByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToDto).ToList();
        }

        public async Task<(bool Success, string UserId)> CreateAsync(CreateUserDto userDto)
        {
            var user = new ApplicationUser
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                PhoneNumber = userDto.PhoneNumber
            };

            var result = await _userRepository.CreateAsync(user, userDto.Password);
            return (result, user.Id);
        }

        public async Task<bool> UpdateAsync(string id, UpdateUserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.PhoneNumber = userDto.PhoneNumber;

            return await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<(bool Success, string? Token)> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.UserName);
            if (user == null) return (false, null);

            var isPasswordValid = await _userRepository.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid) return (false, null);

            var token = _jwtGenerator.CreateToken(user);
            return (true, token);
        }

        private UserDto MapToDto(ApplicationUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };
        }
    }
}
