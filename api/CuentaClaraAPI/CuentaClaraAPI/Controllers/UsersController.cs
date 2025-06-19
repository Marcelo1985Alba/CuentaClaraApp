using CuentaClara.API.Extensions;
using CuentaClara.Application.DTOs;
using CuentaClara.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

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
        [Authorize] //(Roles = "Admin")
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return this.OkResult(result.Users, "Usuarios obtenidos correctamente");
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _userService.GetByIdAsync(id);
            if (!result.Success) return this.NotFoundResult(result.ErrorMessage);

            return this.OkResult(result.userDtoDetails);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(CreateUserDto createUserDto)
        {
            var result = await _userService.CreateAsync(createUserDto);

            if (!result.Success) return this.BadRequestResult("Error al registrar el usuario");

            return this.CreatedResult("GetById", result.UserId, createUserDto);
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
                Secure = true,   // Solo se enviará en conexiones HTTPS
                SameSite = SameSiteMode.None, // Permite enviar cookies entre dominios distintos (CORS)
                Expires = DateTime.UtcNow.AddHours(1),  // Expira en 1 hora
                //Domain = "localhost"  // Agrega esta línea
            };

            Response.Cookies.Append("AuthToken", result.Token, cookieOptions);

            //result.result.User.Token = result.Token;
            return this.OkResult(result.result.User, "Inicio de sesión exitoso");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // 1. Obtener el token de la cookie
                var token = Request.Cookies["authToken"]; // o el nombre que uses

                // 2. Invalidar el token en tu sistema (opcional pero recomendado)
                if (!string.IsNullOrEmpty(token))
                {
                    // Agregar token a una lista negra o marcarlo como inválido en BD
                    _userService.LogOutAsync(token);
                }

                // 3. Borrar la cookie del servidor
                Response.Cookies.Delete("authToken", new CookieOptions
                {
                    Path = "/",
                    Domain = "tudominio.com", // opcional
                    Secure = true,  // si usas HTTPS
                    SameSite = SameSiteMode.Strict
                });

                // 4. También borrar otras cookies relacionadas si las hay
                Response.Cookies.Delete("refreshToken");

                return Ok(new { success = true, message = "Logout exitoso" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Error en logout" });
            }
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

        [HttpGet("debug-auth")]
        public IActionResult DebugAuth()
        {
            var token = Request.Cookies["AuthToken"];
            var cookieExists = !string.IsNullOrEmpty(token);
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
            var username = User.Identity?.Name;

            // Intentar decodificar el token manualmente
            object tokenDetails = "No se pudo decodificar";
            if (cookieExists)
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);
                    tokenDetails = new
                    {
                        jwtToken.ValidFrom,
                        jwtToken.ValidTo,
                        jwtToken.Issuer,
                        Audience = jwtToken.Audiences.FirstOrDefault()
                    };
                }
                catch (Exception ex)
                {
                    tokenDetails = $"Error: {ex.Message}";
                }
            }

            return Ok(new
            {
                CookieExists = cookieExists,
                IsAuthenticated = isAuthenticated,
                Username = username,
                TokenDetails = tokenDetails,
                CurrentTime = DateTime.UtcNow // Para comparar con las fechas del token
            });
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
