namespace HealthCareProject_MVC.Models
{
    public class AppointmentDTO
    {
        public AppointmentModelClass appointment { get; set; }
        public DoctorViewModel doctor { get; set; }
        public RegisterViewModel register { get; set; }

    }
}
