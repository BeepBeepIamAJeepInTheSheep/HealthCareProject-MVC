using HealthCareProject_MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Net.Http;
using System;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace HealthCareProject_MVC.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IConfiguration _configuration;
        public static int tmpVariable { get; set; }
        public DoctorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            List<AppointmentModelClass> appointments = new();
            List<RegisterViewModel> userInfo = new();
            List<PatientDetails> patientDetails = new();
            RegisterViewModel doctorName = new();
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                // client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                int id = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var res3 = await client.GetAsync($"Doctor/GetDoctorDetailsByUserId/{id}");
                

                if (res3.IsSuccessStatusCode)
                {
                    var temData = await res3.Content.ReadAsAsync<DoctorViewModel>();
                    var result = await client.GetAsync($"Doctor/GetAllAppointments/{temData.Id}");
                    var reg = await client.GetAsync("Admin/GetAllTheUserDetails");
                    var res1 = await client.GetAsync("Patient/GetAllPatientDetails");
                    appointments = await result.Content.ReadAsAsync<List<AppointmentModelClass>>();
                    userInfo = await reg.Content.ReadAsAsync<List<RegisterViewModel>>();
                    patientDetails = await res1.Content.ReadAsAsync<List<PatientDetails>>();
                    doctorName = await res3.Content.ReadAsAsync<RegisterViewModel>();
                    

                }
       
                
                   
               
            }

            AdminAppointmentDTO adminAppointmentDTO = new AdminAppointmentDTO()
            {
                Appointment = appointments,
                RegisterView = userInfo,
                PatientDetails = patientDetails,
                registerDetails = doctorName
                
            };



            return View(adminAppointmentDTO);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);

                var UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var doctordetails = await client.GetAsync($"Patient/GetUserDetailsUsingId/{UserId}");
                var docs = await doctordetails.Content.ReadAsAsync<RegisterViewModel>();
                var res = await client.GetAsync("Admin/GetAllTheUserDetails");
                var result = await res.Content.ReadAsAsync<List<RegisterViewModel>>();

                AdminAppointmentDTO adminAppointmentDTO = new AdminAppointmentDTO()
                {
                    registerDetails = docs,
                    RegisterView = result,
                    TempVariable = id,

                };
                tmpVariable = id;

                return View(adminAppointmentDTO);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AdminAppointmentDTO model)
        {
            PatientReport patientReport = new();
            AdminAppointmentDTO user = new AdminAppointmentDTO()
            {
                PatientReport = patientReport
            };

            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();

                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);

                    user.PatientReport.UserId = model.PatientReport.UserId;
                    user.PatientReport.DoctorName = model.PatientReport.DoctorName;
                    user.PatientReport.Medicines = model.PatientReport.Medicines;
                    user.PatientReport.Diognosis = model.PatientReport.Diognosis;
                    user.PatientReport.Symptoms = model.PatientReport.Symptoms;

                    var result = await client.PostAsJsonAsync("Doctor/CreateReport", user.PatientReport);
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }

                }
            }
            RegisterViewModel registerView = new();
            List<RegisterViewModel> registerView1 = new List<RegisterViewModel>();
            AdminAppointmentDTO adminAppointmentDTO = new AdminAppointmentDTO()
            {
                registerDetails = registerView,
                RegisterView = registerView1,
                TempVariable = tmpVariable
            };
            return View(adminAppointmentDTO);
        }

    }
}
