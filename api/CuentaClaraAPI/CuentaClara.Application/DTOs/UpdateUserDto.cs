namespace CuentaClara.Application.DTOs
{
    public class UpdateUserDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string ImageUrl { get; internal set; }
    }
}
