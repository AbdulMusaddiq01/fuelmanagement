using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using FuelManagement.Models;
using FuelmanagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FuelManagement.Pages.Tanks
{
    public class create_tankModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public string ErrorMessage { get; set; }

        public class Tank
        {
            public int id { get; set; }
            public string? code { get; set; }

            [Required]
            [Display(Name = "Fuel type")]
            public string fuel_type { get; set; }

            [Required]
            [Display(Name = "Tank Size")]
            public int tank_size { get; set; }

            [Display(Name = "UOM")]
            public string? uom { get;set; }
            public decimal available_qty { get; set; }
        }


        [BindProperty(SupportsGet = true)]
        public Tank tank { get; set; } 


        public create_tankModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        

        public async Task OnGet(int id)
        {
            if (id <= 0)
            {
                tank = new Tank();
            }
            else
            {
                await getTankdetails(id);
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
                var tankdata = new { id = tank.id, code = tank.code, fuel_type = tank.fuel_type, tank_size = tank.tank_size, uom = tank.uom, available_qty = tank.available_qty };
                var client = _httpClientFactory.CreateClient("API");

                var request = new DBconnect
                {
                    param1 = JsonSerializer.Serialize(tankdata),
                    param2 = "",
                    param3 = "",
                    param4 = "",
                    search = "",
                    operation = "",
                    screen = ""
                };

                HttpResponseMessage response;
                if (tank.id <= 0)
                {
                    response = await client.PostAsJsonAsync("api/Tanks/create", request);
                }
                else
                {
                    response = await client.PutAsJsonAsync($"api/Tanks/update/{tank.id}", request);
                }

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var response_ = JsonSerializer.Deserialize<List<DbResult>>(data);
                    var dbresult = response_?.FirstOrDefault().DBresult;
                    if(dbresult > 0)
                    {
                        return RedirectToPage("./Index");
                    }
                }
                else
                {
                    return Page();
                }
            }
            return Page();
        }
        
        public async Task<IActionResult> getTankdetails(int id)
        {
            var client = _httpClientFactory.CreateClient("API");
            HttpResponseMessage response = await client.GetAsync($"api/Tanks/view/{id}");

            if (response.IsSuccessStatusCode)
            {
                var dbresult = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<Tank>>(dbresult);

                tank = data?.FirstOrDefault();
            }

            return Page();
        }
    }
}
