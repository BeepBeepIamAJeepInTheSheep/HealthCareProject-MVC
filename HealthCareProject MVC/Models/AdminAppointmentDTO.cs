using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace HealthCareProject_MVC.Models
{
    public class AdminAppointmentDTO
    {
        public List<RegisterViewModel> RegisterView { get; set; }
        public List<AppointmentModelClass> Appointment { get; set; }
        public string SelectedValue { get; set; }
        public IEnumerable<SelectListItem> Values { get; set; }
        public RegisterViewModel registerDetails { get; set; }
        public AppointmentModelClass appointmentDetails { get; set; }
        public List<PatientDetails> PatientDetails { get; set; }
        public List<DoctorViewModel> DoctorDetails { get; set; }
        public int TempVariable { get; set; }
        public PatientReport PatientReport { get; set; }
        public List<PatientReport> patientReports { get; set; }
         
    }
}
