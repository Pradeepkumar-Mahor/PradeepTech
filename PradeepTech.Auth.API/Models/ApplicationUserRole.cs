using Microsoft.AspNetCore.Identity;

namespace PradeepTech.Auth.API.Models
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public virtual ApplicationUser User { get; set; }

        public virtual ApplicationRole Role { get; set; }

        public DateTime DateAssigned { get; set; } = DateTime.UtcNow;
    }
}