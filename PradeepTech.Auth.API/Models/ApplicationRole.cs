using Microsoft.AspNetCore.Identity;

namespace PradeepTech.Auth.API.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

        public virtual ICollection<ApplicationRoleClaim> RoleClaims { get; set; }
    }
}