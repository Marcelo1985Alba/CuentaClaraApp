namespace CuentaClara.Application.DTOs
{
    public class UpdateUserDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string ImageUrl { get; set; }

        public string[] Roles { get; set; } = [];
    }
}
