using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using HealthCareProject_MVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System;

namespace HealthCareProject_MVC.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IConfiguration _configuration;
        public AccountsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {

            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    client.BaseAddress = new System.Uri(_configuration["ApiUrl:api"]);
                    var result = await client.PostAsJsonAsync("Accounts/Login", login);
                    // var validStudent = _dbContext.Users.Any(u => u.Name == loginViewModel.Username && u.Password == loginViewModel.Password);

                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string token = await result.Content.ReadAsAsync<string>();
                        HttpContext.Session.SetString("token", token);
                        var userDetails = await client.GetAsync($"Accounts/GetUserDetailsUsingEmail/{login.Email}");
                        var userDetail = await userDetails.Content.ReadAsAsync<RegisterViewModel>();
                        HttpContext.Session.SetString("UserId", userDetail.Id.ToString());

                        string role = await ExtractRole();

                        string roleName = role;
                        HttpContext.Session.SetString("role", roleName);

                        if (role == "ADMIN")
                        {


                            return RedirectToAction("Dashboard", "Admin");

                        }
                        else if (role == "Doctor")
                        {
                            return RedirectToAction("Index", "Doctor");

                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");

                        }
                    }
                    ModelState.AddModelError("", "Invalid Username or Password");
                }
            }
            TempData["Alert"] = "Invalid login credentials. Please try again.";
            return View(login);
        }

        [NonAction]
        public async Task<string> ExtractRole()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                client.BaseAddress = new System.Uri(_configuration["Apiurl:api"]);
                var roleResult = await client.GetAsync("Accounts/GetRole");
                if (roleResult.IsSuccessStatusCode)
                {
                    var role = await roleResult.Content.ReadAsAsync<string>();
                    return role;
                }
                return null;
            }
        }

        [HttpPost]
        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("token");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();

                    client.BaseAddress = new Uri(_configuration["ApiUrl:api"]);


                    RegisterViewModel user = new RegisterViewModel
                    {
                        Name = model.Name,
                        Password = model.Password,
                        ConfirmPassword = model.ConfirmPassword,
                        Email = model.Email,
                        Role = "Patient",
                        PhoneNumber = model.PhoneNumber
                    };

                    var result = await client.PostAsJsonAsync("Accounts/Register", user);
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Login");
                    }

                }
            }
            RegisterViewModel user2 = new RegisterViewModel
            {
                Name = model.Name,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                Email = model.Email,
                Role = "Patient"
            };
            return View(user2);
        }
    }
}
