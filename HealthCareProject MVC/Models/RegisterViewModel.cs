using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareProject_MVC.Models
{

    public class RegisterViewModel 
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter your name")]
        [MaxLength(50)]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "User name should be minimum of 3 characters and maximum of 50 characters")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage ="Please enter the phone number")]
        [Phone(ErrorMessage = "Please enter a correct phone number")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Please enter a correct phone number (10 digits)")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please enter the password")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Password is too short.")]
        [PasswordPropertyText]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter the password again")]
        [Compare("Password", ErrorMessage = "Password does not match")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Password is too short.")]
        [PasswordPropertyText]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string Role { get; set; }

    }
}
