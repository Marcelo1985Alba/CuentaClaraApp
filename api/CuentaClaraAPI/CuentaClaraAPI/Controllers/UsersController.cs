using CuentaClara.API.Extensions;
using CuentaClara.Application.DTOs;
using CuentaClara.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CuentaClara.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return this.OkResult(result, "Usuarios obtenidos correctamente");
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (!result.Success) return this.NotFoundResult(result.ErrorMessage);

            return this.OkResult(result.Item2);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(CreateUserDto createUserDto)
        {
            var result = await _userService.CreateAsync(createUserDto);

            if (!result.Success) return this.BadRequestResult("Error al registrar el usuario");

            return this.CreatedResult("Usuario registrado exitosamente", result.UserId, createUserDto);
            //return CreatedAtAction(nameof(GetById), new { id = result.UserId }, null);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _userService.LoginAsync(loginDto);

            if (!result.result.Success) return this.UnauthorizedResult("Credenciales inválidas");

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // La cookie no será accesible desde JavaScript
                Secure = false,   // Solo se enviará en conexiones HTTPS
                SameSite = SameSiteMode.Lax, // Permite enviar cookies entre dominios distintos (CORS)
                Expires = DateTime.UtcNow.AddHours(1) // Expira en 1 hora
                //Domain = "localhost"  // Agrega esta línea
            };

            Response.Cookies.Append("AuthToken", result.Token, cookieOptions);

            //result.result.User.Token = result.Token;
            return this.OkResult(result.result.User, "Inicio de sesión exitoso");
        }

        [HttpGet("UserLoggedIn")]
        [Authorize]
        public IActionResult GetUserLoggedIn()
        {
            var token = Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
                return this.UnauthorizedResult("Credenciales inválidas");


            return this.OkResult(new UserDto());

            //return Ok(new { Message = "Bienvenido!", Token = token });
        }


        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, UpdateUserDto updateUserDto)
        {
            var result = await _userService.UpdateAsync(id, updateUserDto);

            if (!result.Success) return  this.NotFoundResult($"Usuario con ID {id} no encontrado");

            return this.OkResult();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteAsync(id);

            if (!result.Success) return this.NotFoundResult($"Usuario con ID {id} no encontrado");

            return this.NoContent();
        }
    }
}
