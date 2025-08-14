using Microsoft.AspNetCore.Identity;

namespace PradeepTech.Auth.API.Models
{
    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public int Id { get; set; }

        public string RoleId { get; set; } // Ensure this matches the PK type of ApplicationRole

        public ApplicationRole Role { get; set; }

        // public virtual ApplicationRole Role { get; set; }
    }
}