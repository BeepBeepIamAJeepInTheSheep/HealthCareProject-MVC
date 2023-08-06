using HealthCareProject_MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HealthCareProject_MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _configuration;
        public static DoctorViewModel ViewModel = new();

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            DoctorDTO dto = new();
            dto.Doctors = new();
            dto.Doctors2 = new();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var result = await client.GetAsync($"Admin/GetAllDoctorsUsingRole");
                var result2 = await client.GetAsync($"Admin/GetAllDoctors");
                if (result.IsSuccessStatusCode)
                {
                    dto.Doctors = await result2.Content.ReadAsAsync<List<DoctorViewModel>>();
                    dto.Doctors2 = await result.Content.ReadAsAsync<List<RegisterViewModel>>();
                    return View(dto);
                }

            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {

            DoctorViewModel doctor = new();
            using (var client = new HttpClient())
            {

                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var doctorDetails = await client.GetAsync($"Admin/GetDoctorById/{id}");

                // var houseAndSellerDetails = await client.GetAsync($"Seller/GetSellerDetails/{id}");


                if (doctorDetails.IsSuccessStatusCode)
                {
                    doctor = await doctorDetails.Content.ReadAsAsync<DoctorViewModel>();
                    return View(doctor);
                }
                doctor.UserId = id;
                return View(doctor);
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegisterViewModel staff)
        {
            RegisterViewModel user = new();
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                    staff.Role = "Doctor";
                    var result = await client.PostAsJsonAsync("Accounts/Register", staff);

                    if (result.IsSuccessStatusCode)
                    {
                        // cityViewModels = await cities.Content.ReadAsAsync<List<CityViewModel>>();
                        return RedirectToAction("Index", "Admin");
                    }
                }
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult AddDetails(int id)
        {
            ViewModel.UserId = id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddDetails(DoctorViewModel staff)
        {
            DoctorViewModel doctor = new();
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    ViewModel.Gender = staff.Gender;
                    ViewModel.Age = staff.Age;
                    ViewModel.Education = staff.Education;
                    ViewModel.Experience = staff.Experience;
                    ViewModel.Fees = staff.Fees;
                    ViewModel.Specialization = staff.Specialization;
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PostAsJsonAsync("Admin/AddDoctor", ViewModel);

                    if (result.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        // cityViewModels = await cities.Content.ReadAsAsync<List<CityViewModel>>();
                        return RedirectToAction("Index", "Admin");
                    }
                }
            }
            return View(doctor);
        }


        [AcceptVerbs("GET", "POST")]

        public async Task<IActionResult> AppointmentIndex([FromForm] string selectedValue)
        {
            List<DoctorViewModel> doctors = new();
            List<AppointmentModelClass> appointments = new();
            List<RegisterViewModel> userInfo = new();
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);

                // client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);

                if (string.IsNullOrEmpty(selectedValue))
                {
                    selectedValue = "All";
                }
                var res = await client.GetAsync("Admin/GetAllDoctors");
                doctors = await res.Content.ReadAsAsync<List<DoctorViewModel>>();
                var result = await client.GetAsync($"Admin/GetAllAppointments/{selectedValue}");
                var reg = await client.GetAsync("Admin/GetAllTheUserDetails");

                if (result.IsSuccessStatusCode)
                {
                    appointments = await result.Content.ReadAsAsync<List<AppointmentModelClass>>();
                    userInfo = await reg.Content.ReadAsAsync<List<RegisterViewModel>>();

                }
            }

            AdminAppointmentDTO adminAppointmentDTO = new AdminAppointmentDTO()
            {
                Appointment = appointments,
                RegisterView = userInfo,
                DoctorDetails = doctors,
                Values = new List<SelectListItem>
                        {
                            new SelectListItem { Value = "All", Text = "All" },
                            new SelectListItem { Value = "REJECTED", Text = "Rejected" },
                            new SelectListItem { Value = "APPROVED", Text = "Approved" },
                            new SelectListItem { Value = "PENDING", Text = "Pending" }
                        },
            };



            return View(adminAppointmentDTO);
        }



        public async Task<IActionResult> AppointmentDetails(int id)
        {
            AppointmentModelClass appointments = new();
            List<RegisterViewModel> userInfo = new();
            List<DoctorViewModel> doctors = new();

            using (var client = new HttpClient())
            {

                client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                var appointmentDetails = await client.GetAsync($"Admin/GetAppointmentdetails/{id}");

                // var houseAndSellerDetails = await client.GetAsync($"Seller/GetSellerDetails/{id}");
                var appointmentDet = new AppointmentModelClass();

                if (appointmentDetails.IsSuccessStatusCode)
                {

                    appointmentDet = await appointmentDetails.Content.ReadAsAsync<AppointmentModelClass>();

                    var Users = await client.GetAsync($"Admin/GetAllTheUserDetails");

                    userInfo = await Users.Content.ReadAsAsync<List<RegisterViewModel>>();

                    var res = await client.GetAsync("Admin/GetAllDoctors");
                    doctors = await res.Content.ReadAsAsync<List<DoctorViewModel>>();
                    
                }
                AdminAppointmentDTO adminAppointment = new AdminAppointmentDTO()
                {
                    appointmentDetails = appointmentDet,
                    RegisterView = userInfo,
                    DoctorDetails= doctors
                };
                return View(adminAppointment);
            }

        }

        public async Task<IActionResult> ApproveAppointment(int id)
        {

            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);
                var res = await client.PutAsync($"Admin/ApproveAppointment/{id}", null);
                if (res.IsSuccessStatusCode)
                {

                    return RedirectToAction("AppointmentIndex", "Admin");
                }



                return BadRequest();
            }
        }

        public async Task<IActionResult> RejectAppointment(int id)
        {

            using (var client = new HttpClient())
            {


                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);

                //  client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);

                var res = await client.PutAsync($"Admin/RejectAppointment/{id}", null);
                if (res.IsSuccessStatusCode)
                {
                    return RedirectToAction("AppointmentIndex", "Admin");
                }



                return BadRequest();
            }
        }

        public IActionResult Dashboard()
        {
            return View();
        }

    }
}

