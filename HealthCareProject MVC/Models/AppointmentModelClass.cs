using System.ComponentModel.DataAnnotations;
using System;

namespace HealthCareProject_MVC.Models
{
    public class AppointmentModelClass
    {
     
        public int Id { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }

        
        public DateTime AppointmentDate { get; set; }
        public int DoctorsId { get; set; }
        public int UserId { get; set; }
    }
}
