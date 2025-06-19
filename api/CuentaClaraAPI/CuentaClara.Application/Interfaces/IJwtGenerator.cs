using CuentaClara.Domain.Entities;
using System.Security.Claims;

namespace CuentaClara.Application.Interfaces
{
    public interface IJwtGenerator
    {
        string CreateToken(ApplicationUser user);
        ClaimsPrincipal ValidateToken(string token);

        void InvalidateToken(string token);

    }
}
