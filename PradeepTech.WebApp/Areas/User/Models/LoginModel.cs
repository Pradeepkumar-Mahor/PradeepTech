using System.ComponentModel.DataAnnotations;

namespace PradeepTech.WebApp.Areas.User.Models
{
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}