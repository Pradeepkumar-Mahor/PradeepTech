using Microsoft.AspNetCore.Identity;

namespace PradeepTech.Auth.API.Models
{
    public class UserClaim : IdentityUserClaim<string>
    {
        public virtual ApplicationUser User { get; set; }
    }
}