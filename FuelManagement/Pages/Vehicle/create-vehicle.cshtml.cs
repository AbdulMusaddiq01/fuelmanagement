using FuelmanagementApi.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using FuelManagement.Models;

namespace FuelManagement.Pages.Vehicle
{
    public class create_vehicleModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public string ErrorMessage { get; set; }

        public class Vehicle
        {
            public int id { get; set; }
            public string? code { get; set; }

            [Required]
            [Display(Name = "Vehicle Number")]
            public string vehicle_number { get; set; }

            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "Tank size must be greater than 0")]
            [Display(Name = "Tank Size")]
            public int tank_size { get; set; }

            [Required]
            [Display(Name = "Driver Name")]
            public string driver_name { get; set; }

            [Display(Name = "Odometer Reading")]
            public string odometer_reading { get; set; }
            public decimal available_qty { get; set; }
        }


        [BindProperty(SupportsGet = true)]
        public Vehicle vehicle { get; set; }


        public create_vehicleModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        public async Task OnGet(int id)
        {
            if (id <= 0)
            {
                vehicle = new Vehicle();
            }
            else
            {
                await getVehicledetails(id);
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
                var tankdata = new { id = vehicle.id, vehicle_number = vehicle.vehicle_number, tank_size = vehicle.tank_size, driver_name = vehicle.driver_name, odometer_reading = vehicle.odometer_reading };
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
                if (vehicle.id <= 0)
                {
                    response = await client.PostAsJsonAsync("api/Vehicle/create-vehicle", request);
                }
                else
                {
                    response = await client.PutAsJsonAsync($"api/Vehicle/update-vehicle/{vehicle.id}", request);
                }

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var response_ = JsonSerializer.Deserialize<List<DbResult>>(data);
                    var dbresult = response_?.FirstOrDefault().DBresult;
                    if (dbresult > 0)
                    {
                        return RedirectToPage("./Index");
                    }else if (dbresult < 0)
                    {
                        ErrorMessage = $"Vehicle with {vehicle.vehicle_number} already exist";
                        return Page();
                    }
                }
                else
                {
                    return Page();
                }
            }
            return Page();
        }

        public async Task<IActionResult> getVehicledetails(int id)
        {
            var client = _httpClientFactory.CreateClient("API");
            HttpResponseMessage response = await client.GetAsync($"api/Vehicle/view-vehicle/{id}");

            if (response.IsSuccessStatusCode)
            {
                var dbresult = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<Vehicle>>(dbresult);

                vehicle = data?.FirstOrDefault();
            }

            return Page();
        }
    }
}
