using Microsoft.AspNetCore.Identity;

namespace PradeepTech.Auth.API.Models
{
    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public int Id { get; set; }

        public virtual ApplicationRole Role { get; set; }
    }
}