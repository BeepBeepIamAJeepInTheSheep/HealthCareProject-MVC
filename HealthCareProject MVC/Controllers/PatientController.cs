using HealthCareProject_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Reflection;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Numerics;

namespace HealthCareProject_MVC.Controllers
{
    public class PatientController : Controller
    {
        private readonly IConfiguration _configuration;
        public static AppointmentDTO appointment = new();


        public PatientController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Book(int id)
        {

            DoctorViewModel doctorViewModel = new();
            RegisterViewModel registerViewModel = new();
            AppointmentModelClass ModelClass = new();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                appointment.doctor = doctorViewModel;
                appointment.appointment = ModelClass;
                appointment.register = registerViewModel;

                appointment.appointment.DoctorsId = id;
                var userdoctorId = await client.GetAsync($"Doctor/GetDoctorDetailsById/{id}");
                var userDocId = await userdoctorId.Content.ReadAsAsync<DoctorViewModel>();
                var doctorDetails = await client.GetAsync($"Admin/GetDoctorById/{userDocId.UserId}");
                appointment.doctor = await doctorDetails.Content.ReadAsAsync<DoctorViewModel>();

                var doctorDetails2 = await client.GetAsync($"Patient/GetUserDetailsUsingId/{appointment.doctor.UserId}");

                if (doctorDetails2.IsSuccessStatusCode)
                {
                    appointment.register = await doctorDetails2.Content.ReadAsAsync<RegisterViewModel>();
                    return View(appointment);
                }
                else
                {
                    return RedirectToAction("Login", "Accounts");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Book([FromForm] AppointmentDTO appointment1)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    appointment1.appointment.Status = "PENDING";
                    var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    appointment1.appointment.UserId = userId;
                    appointment1.appointment.DoctorsId = appointment.appointment.DoctorsId;
                    client.DefaultRequestHeaders.Clear();
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);

                    var result = await client.PostAsJsonAsync("Patient/CreateAppointment", appointment1.appointment);
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("BookIndex");
                    }
                    else
                    {
                        return RedirectToAction("Login","Accounts");

                    }
                }
            }

            return View(appointment1);
        }


        [HttpGet]
        public async Task<IActionResult> BookIndex()
        {

            AdminAppointmentDTO adminAppointmentDTO = new AdminAppointmentDTO()
            {
                RegisterView = new(),
                DoctorDetails = new()
            };
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync($"Admin/GetAllDoctorsUsingRole");
                var res2 = await client.GetAsync("Admin/GetAllDoctors");
                if (result.IsSuccessStatusCode)
                {
                    adminAppointmentDTO.DoctorDetails = await res2.Content.ReadAsAsync<List<DoctorViewModel>>();
                    adminAppointmentDTO.RegisterView = await result.Content.ReadAsAsync<List<RegisterViewModel>>();
                    return View(adminAppointmentDTO);
                }

            }
            return View(adminAppointmentDTO);
        }

        [HttpGet]
        public async Task<IActionResult> AddUserDetails()
        {
            PatientDetails patientDetails = new();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var result = await client.GetAsync($"Patient/GetPatientDetailsUsingId/{userId}");
                if (result.IsSuccessStatusCode)
                {
                    patientDetails = await result.Content.ReadAsAsync<PatientDetails>();
                }

            }
            return View(patientDetails);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserDetails([FromForm] PatientDetails patientDetails, int id)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                if (id == 0)
                {
                    var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    patientDetails.UsersId = userId;
                    var result = await client.PostAsJsonAsync($"Patient/CreatePatientDetails", patientDetails);
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Accounts");
                    }
                }
                else
                {
                    var userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                    patientDetails.UsersId = userId;
                    var result = await client.PutAsJsonAsync($"Patient/UpdatePatientDetails/{userId}", patientDetails);
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View(patientDetails);
                    }
                }

            }
        }

        [HttpGet]
        public async Task<IActionResult> GetReport()
        {
            AdminAppointmentDTO admin = new();
            List<PatientReport> patientReport = new();
            RegisterViewModel register = new();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                var id = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var res = await client.GetAsync($"Patient/GetAllPatientReports/{id}");
                var result = await client.GetAsync($"Patient/GetUserDetailsUsingId/{id}");
                if (result.IsSuccessStatusCode)
                {
                    patientReport = await res.Content.ReadAsAsync<List<PatientReport>>();
                    register = await result.Content.ReadAsAsync<RegisterViewModel>();
                    admin.patientReports = patientReport;
                    admin.registerDetails = register;
                    return View(admin);
                }

            }
            admin.patientReports = patientReport;
            admin.registerDetails = register;
            return View(admin);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyAppointment()
        {
            List<AppointmentModelClass> appointmentModels = new();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                var id = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
                var res = await client.GetAsync($"Patient/GetMyAppointments/{id}");
                if (res.IsSuccessStatusCode)
                {
                    appointmentModels = await res.Content.ReadAsAsync<List<AppointmentModelClass>>();
                    
                    return View(appointmentModels);
                }

            }
            
            return View(appointmentModels);
        }
    }
}
