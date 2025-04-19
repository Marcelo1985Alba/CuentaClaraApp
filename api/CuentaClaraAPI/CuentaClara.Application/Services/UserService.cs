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

        public async Task<(bool Success, UserDto?, string? ErrorMessage)> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return (false, null, "El id de usuario es requerido");

            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                return (false, null, "Usuario no encontrado");

            return (true, MapToDto(user), null);
        }

        public async Task<(bool Success, UserDto?, string? ErrorMessage)> GetByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                return (false, null, "El email de usuario es requerido");

            var user = await _userRepository.GetByEmailAsync(email);

            if (user == null)
                return (false, null, "Usuario no encontrado");

            return (true, MapToDto(user), null);
        }

        public async Task<(bool Success, IEnumerable<UserDto> Users, string? ErrorMessage)> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            if (users == null || !users.Any())
                return (false, Enumerable.Empty<UserDto>(), "No se encontraron usuarios");


            return (true, users.Select(MapToDto).ToList(), null);
        }

        public async Task<(bool Success, string? UserId, string? ErrorMessage)> CreateAsync(CreateUserDto userDto)
        {
            var user = new ApplicationUser
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                PhoneNumber = userDto.PhoneNumber,
                ImageUrl = userDto.ImageUrl
            };

            var result = await _userRepository.CreateAsync(user, userDto.Password);
            if (!result)
            {
                var errorMessage = string.Join(", ", "Error al crear usuario");
                return (false, null, errorMessage);
            }
            return (result, user.Id, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(string id, UpdateUserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return (false, "Usuario no encontrado");

            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.PhoneNumber = userDto.PhoneNumber;
            user.ImageUrl = userDto.ImageUrl;

            var result = await _userRepository.UpdateAsync(user);
            if (!result)
                return (false, "Error al actualizar usuario");

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(string id)
        {
            var result = await _userRepository.DeleteAsync(id);
            if (!result)
                return (false, "Error al eliminar usuario");

            return (true, null);
        }

        public async Task<((bool Success, UserDto? User, string? ErrorMessage) result, string? Token)> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.UserName);
            if (user == null)
                return ((false, null, "Usuario no encontrado"), null);

            var isPasswordValid = await _userRepository.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordValid)
                return ((false, null, "Contraseña incorrecta"), null);

            var token = _jwtGenerator.CreateToken(user);

            var userDto= MapToDto(user);
            return ((true, userDto, null), token);
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
                PhoneNumber = user.PhoneNumber,
                ImageUrl = user.ImageUrl
            };
        }
    }
}
