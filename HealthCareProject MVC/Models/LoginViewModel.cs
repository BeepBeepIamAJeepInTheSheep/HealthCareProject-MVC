using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HealthCareProject_MVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter the email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [PasswordPropertyText]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
