
namespace CuentaClara.Domain.Entities
{
    public class ApplicationUser
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        // Propiedades adicionales personalizadas
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        // Relación con roles
        public virtual ICollection<string> Roles { get; set; } = new HashSet<string>();
    }
}
