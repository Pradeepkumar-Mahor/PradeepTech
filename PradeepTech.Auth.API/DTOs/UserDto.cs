namespace PradeepTech.Auth.API.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? LastLogin { get; set; }

        public List<string> Roles { get; set; } = new();

        public List<string> Claims { get; set; } = new();
    }
}