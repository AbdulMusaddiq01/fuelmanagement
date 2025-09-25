using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using FuelmanagementApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Tls;

namespace FuelManagement.Pages
{
    public class loginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public loginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public class Loginrequest
        {
            [Required(ErrorMessage = "Username is required")] 
            public string Username { get; set; }

            [Required(ErrorMessage = "Password is required")]
            public string Password { get; set; }
        }

        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Role { get; set; }
            public string Status { get; set; }

            public object Is_Deleted { get; set; }
            public DateTime Created_Date { get; set; }
            public DateTime Updated_Date { get; set; }
            public int DBresult { get; set; }
        }
        public class ApiResponse
        {
            public List<User> Data { get; set; }
            public string Token { get; set; }
        }


        [BindProperty]
        public Loginrequest Login { get; set; }

        [BindProperty]
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostLogin()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else if(ModelState.IsValid)
            {
                var userrequest = new { username = Login.Username, password = Login.Password };
                var client = _httpClientFactory.CreateClient("API");

                var request = new DBconnect
                {
                    param1 = JsonSerializer.Serialize(userrequest),
                    param2 = "",
                    param3 = "",
                    param4 = "",
                    operation = "Login",
                    search = "",
                    screen = "LoginUsers"

                };

                var response = await client.PostAsJsonAsync("api/User/login", request);
                if (response.IsSuccessStatusCode)
                {
                    var dbresult = await response.Content.ReadAsStringAsync();
                    var unwrap  = JsonSerializer.Deserialize<ApiResponse>(dbresult, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true});


                    var response_ = unwrap.Data;
                    if (response_ != null && response_.Count > 0)
                    {
                        var dbResponse = response_[0];
                        int dbResult = dbResponse.DBresult;

                        if (dbResult == 1)
                        {
                            HttpContext.Session.SetString("userdetails", JsonSerializer.Serialize(response_[0]));
                            HttpContext.Session.SetString("AuthToken", unwrap.Token);
                            return RedirectToPage("/Fuel/fuel-types");
                        }
                        else if (dbResult == -1 || dbResult == -2)
                        {
                            ErrorMessage = !string.IsNullOrEmpty(dbResponse.Status) ? "Success": "Invalid username or password";
                            return Page();
                        }
                    }

                }
            }
            ErrorMessage = "Something went wrong. Please try again.";
            return Page();
        }
    }
}
