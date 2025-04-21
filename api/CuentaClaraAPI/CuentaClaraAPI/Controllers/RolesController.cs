using CuentaClara.API.Extensions;
using CuentaClara.Application.DTOs.Role;
using CuentaClara.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CuentaClara.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _rolService;
        public RolesController(IRoleService rolService)
        {
            _rolService = rolService;
        }

        [HttpGet]
        [Authorize] //(Roles = "Admin")
        public async Task<IActionResult> GetAll()
        {
            var result = await _rolService.GetAllAsync();
            return this.OkResult(result.Roles, "Roles obtenidos correctamente");
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _rolService.GetByIdAsync(id);
            if (!result.Success) return this.NotFoundResult(result.ErrorMessage);
            return this.OkResult(result.Item2);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(RoleDto role)
        {
            var result = await _rolService.CreateAsync(role);
            if (!result.Success) return this.NotFoundResult(result.ErrorMessage);
            role.Id = result.RoleId;
            return this.CreatedResult("GetById", result.RoleId, role);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id,RoleDto role)
        {
            var result = await _rolService.UpdateAsync(id, role);
            if (!result.Success) return this.NotFoundResult(result.ErrorMessage);
            return this.OkResult();
        }
    }
}
