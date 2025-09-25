using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using FuelManagement.Models;
using FuelmanagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FuelManagement.Pages.Issuers
{
    public class create_issuerModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public string ErrorMessage { get; set; }
        public create_issuerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public class Issuer
        {
            public int id { get; set; }
            [Required]
            [Display(Name = "Issuer Name")]
            public string? issuer_name { get; set; }
            [Required]
            [Display(Name = "Contact")]
            public string? contact { get; set; }
            public int is_deleted { get; set; }
        }

        [BindProperty(SupportsGet = true)]
        public Issuer issuer { get; set; }
        public async Task OnGet(int id)
        {
            if (id == 0)
            {
                issuer = new Issuer();
            }
            else
            {
                await GetIssuerDetails(id);
            }
        }


        public async Task<IActionResult> OnPostSaveAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else
            {
                var issuer_ = new { id = issuer.id, issuer_name = issuer.issuer_name, contact = issuer.contact, is_deleted = issuer.is_deleted };
                var client = _httpClientFactory.CreateClient("API");

                var request = new DBconnect
                {
                    param1 = JsonSerializer.Serialize(issuer_),
                    param2 = "",
                    param3 = "",
                    param4 = "",
                    search = "",
                    operation = "",
                    screen = "Issuers",
                };

                HttpResponseMessage response;
                if (issuer.id > 0)
                {
                    response = await client.PutAsJsonAsync($"api/User/update-issuer/{issuer.id}", request);
                }
                else
                {
                    response = await client.PostAsJsonAsync("api/User/create-issuer", request);
                }

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var response_ = JsonSerializer.Deserialize<List<DbResult>>(data);

                    var dbresult = response_?.FirstOrDefault()?.DBresult;
                    if (dbresult > 0)
                    {
                        return RedirectToPage("./Index");
                    }
                    else
                    {
                        ErrorMessage = $"Fuel with {issuer.issuer_name} already exists!";
                        return Page();
                    }
                }

            }

            return Page();
        }

        public async Task<IActionResult> GetIssuerDetails(int id)
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.GetAsync($"api/User/issuer/{id}");

            if (response.IsSuccessStatusCode)
            {
                var dbresult = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<Issuer>>(dbresult);

                issuer = data?.FirstOrDefault();
            }


            return Page();
        }
    }
}
