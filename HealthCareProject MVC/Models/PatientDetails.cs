using System.ComponentModel.DataAnnotations;

namespace HealthCareProject_MVC.Models
{
    public class PatientDetails
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string BloodGroup { get; set; }
        [Required]
        public double Height { get; set; }
        [Required]
        public double Weight { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Gender { get; set; }
        public int UsersId { get; set; }
    }
}
