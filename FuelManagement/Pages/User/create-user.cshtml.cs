using FuelmanagementApi.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FuelManagement.Pages.User
{
    public class create_userModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public create_userModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public class DbResult
        {
            public int DBresult { get; set; }
            public string? Message { get; set; }
        }

        public string ErrorMessage { get; set; }
        public class LoginUser
        {
            public int? id { get; set; }
            [Required(ErrorMessage = "Name is required.")]
            public string name { get; set; }

            [Required(ErrorMessage = "Username is required.")]
            public string? username { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            public string? password { get; set; }

            [Required(ErrorMessage = "Role is required.")]
            public string? role { get; set; }
            public int is_deleted { get; set; }
        }

        [BindProperty(SupportsGet = true)]
        public LoginUser loginUser { get; set; }
        public async Task OnGet(int id)
        {
            if (id == 0)
            {
                loginUser = new LoginUser();
            }
            else if (id > 0)
            {
               await  ViewIssuerDetails(id);
            }
        }


        public async Task<IActionResult> OnPostSave()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else
            {
                var issuerdata = new
                {
                    id = loginUser.id,
                    name = loginUser.name,
                    username = loginUser.username,
                    password = loginUser.password,
                    role = loginUser.role,
                    is_deleted = loginUser.is_deleted
                };
                var client = _httpClientFactory.CreateClient("API");
                var request = new DBconnect
                {
                    param1 = JsonSerializer.Serialize(issuerdata),
                    param2 = "",
                    param3 = "",
                    param4 = "",
                    search = "",
                    operation = loginUser.id == 0 ? "Insert" : "Update",
                    screen = "LoginUsers"
                };

                var response = await client.PostAsJsonAsync("api/User/create", request);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var response_ = JsonSerializer.Deserialize<List<DbResult>>(data);

                    var dbResult = response_?.FirstOrDefault().DBresult;
                    if (dbResult > 0)
                    {
                        return RedirectToPage("./Index");
                    }
                    else if (dbResult == -1)
                    {
                        ErrorMessage = $"Issuer with this {loginUser.name} already exist";
                        return Page();
                    }
                }
            }
            return RedirectToPage();
        }

        public async Task ViewIssuerDetails(int id)
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.GetAsync($"api/User/view/{id}");
            if (response.IsSuccessStatusCode)
            {
                var dbresult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<LoginUser>>(dbresult);
                loginUser = result?.FirstOrDefault();
            }
        }
    }
}
