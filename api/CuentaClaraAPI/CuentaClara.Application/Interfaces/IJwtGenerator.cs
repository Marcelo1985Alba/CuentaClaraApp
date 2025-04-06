using CuentaClara.Domain.Entities;

namespace CuentaClara.Application.Interfaces
{
    public interface IJwtGenerator
    {
        string CreateToken(ApplicationUser user);
    }
}
