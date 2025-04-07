﻿using CuentaClara.Application.DTOs;
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
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(CreateUserDto createUserDto)
        {
            var result = await _userService.CreateAsync(createUserDto);

            if (!result.Success) return BadRequest("Error al registrar el usuario");

            return CreatedAtAction(nameof(GetById), new { id = result.UserId }, null);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _userService.LoginAsync(loginDto);

            if (!result.Success) return Unauthorized("Credenciales inválidas");

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // La cookie no será accesible desde JavaScript
                Secure = true,   // Solo se enviará en conexiones HTTPS
                SameSite = SameSiteMode.None, // Permite enviar cookies entre dominios distintos (CORS)
                Expires = DateTime.UtcNow.AddHours(1) // Expira en 1 hora
            };

            Response.Cookies.Append("AuthToken", result.Token, cookieOptions);

            //return Ok(new { Token = result.Token });
            return Ok();
        }

        [HttpGet("UserLoggedIn")]
        [Authorize]
        public IActionResult GetUserLoggedIn()
        {
            var token = Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { Message = "No autenticado" });

            return Ok(new { Message = "Bienvenido!", Token = token });
        }


        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, UpdateUserDto updateUserDto)
        {
            var result = await _userService.UpdateAsync(id, updateUserDto);

            if (!result) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteAsync(id);

            if (!result) return NotFound();

            return NoContent();
        }
    }
}
