using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuentaClara.Infrastructure.Models
{
    public class AppUserRole : IdentityRole
    {
        public string Description { get; set; } = null!;
    }
}
