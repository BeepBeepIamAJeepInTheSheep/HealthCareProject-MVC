using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace HealthCareProject_MVC.Components
{
    public class LoginViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration; public LoginViewComponent(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("token"));
                client.BaseAddress = new System.Uri(_configuration["Apiurl:api"]);
                var result = await client.GetAsync("Accounts/GetName");
                var roleResult = await client.GetAsync("Accounts/GetRole");
                if (result.IsSuccessStatusCode && roleResult.IsSuccessStatusCode)
                {
                    var name = await result.Content.ReadAsAsync<string>();
                    ViewBag.Name = name;

                    var role = await roleResult.Content.ReadAsAsync<string>();
                    ViewBag.Role = role;


                    return View("_LoginPartial");
                }
            }
            return View("_LoginPartial");
        }
    }
}
