using System.ComponentModel.DataAnnotations;
using FuelmanagementApi.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FuelManagement.Pages.Fuel
{
    public class createModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public string ErrorMessage { get; set; }

        public class FuelType
        {
            public int? id { get; set; }
            [Required(ErrorMessage = "Name is required")]
            public string Name { get; set; }
            public string? ColorCode { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? UpdateDate { get; set; }
        }

        [BindProperty(SupportsGet = true)]
        public FuelType fuelType { get; set; }

        public class DbResult
        {
            public int DBresult { get; set; }
        }


        // constructor
        public createModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        public async Task OnGet(int id)
        {
            if (id == 0)
            {
                fuelType = new FuelType();

            }else if (id > 0)
            {
                await OnGetEdit(id);
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
                var fuelrequest = new { id = fuelType.id, name = fuelType.Name, color_code = fuelType.ColorCode };
                var client = _httpClientFactory.CreateClient("API");

                var request = new DBconnect
                {
                    param1 = JsonSerializer.Serialize(fuelrequest),
                    param2 = "",
                    param3 = "",
                    param4 = "",
                    operation = fuelType.id > 0 ? "Update" : "Insert",
                    search = "",
                    screen = "FuelTypes"
                };

                var response = await client.PostAsJsonAsync("api/Fuel/fuel-types", request);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var response_ = JsonSerializer.Deserialize<List<DbResult>>(data);

                    var dbResult = response_?.FirstOrDefault()?.DBresult;
                    if (dbResult > 0)
                    {
                        return RedirectToPage("./fuel-types");
                    }
                    else if (dbResult == -1)
                    {
                        ErrorMessage = $"Fuel with {fuelType.Name} already exists!";
                        return Page();
                    }
                }
            }

            return RedirectToPage();
        }

        public async Task OnGetEdit(int id)
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.GetAsync($"api/Fuel/view-fuel-type/{id}");

            if (response.IsSuccessStatusCode)
            {
                var dbresult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<FuelType>>(dbresult);

                fuelType = result?.FirstOrDefault();
                
            }

        }
    }
}
