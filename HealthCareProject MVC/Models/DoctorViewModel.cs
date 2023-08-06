using System.ComponentModel.DataAnnotations;

namespace HealthCareProject_MVC.Models
{
    public class DoctorViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Specialization { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string Education { get; set; }
        [Required]
        public string Experience { get; set; }
        [Required]
        public int Fees { get; set; }
        [Required]
        public string Gender { get; set; }
        public int UserId { get; set; }

    }
}
