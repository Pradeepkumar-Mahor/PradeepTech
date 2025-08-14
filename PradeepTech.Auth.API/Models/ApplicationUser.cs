using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PradeepTech.Auth.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string UserId { get; set; } // Or Guid, int, etc.

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }

        public bool IsActive { get; set; } = true;

        public string? ProfilePicture { get; set; }

        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        // Navigation properties
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

        public virtual ICollection<UserClaim> UserClaims { get; set; }
    }
}