using FuelmanagementApi.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using FuelManagement.Models;
using static FuelManagement.Pages.issue_fuel.issue_fuelModel;
using static FuelManagement.Pages.Tanks.create_tankModel;
using System.ComponentModel.DataAnnotations;
using MySqlX.XDevAPI;

namespace FuelManagement.Pages.issue_fuel
{
    public class issue_fuelModel : PageModel
    {

        private readonly IHttpClientFactory _httpClientFactory;
        public string ErrorMessage { get; set; }
        public List<Dictionary<string, object>> tanks { get; set; } = new();
        public List<Dictionary<string, object>> vehicles { get; set; } = new();

        public class IssueFuel
        {
            public int id { get; set; }

            [Required(ErrorMessage = "Please select a tank.")]
            [Display(Name = "Tank")]
            public int tank_id { get; set; }

            [Display(Name = "Issuer")]
            public int issuer_id { get; set; }

            [Required(ErrorMessage = "Please select the issue date.")]
            [Display(Name = "Issue Date")]
            public DateTime? issue_date { get; set; }

            [Display(Name = "Fuel Type")]
            public string? fuel_type { get; set; }

            [Required(ErrorMessage = "Please specify who the fuel is issued to.")]
            [Display(Name = "Issued To Type")]
            public string issued_to_type { get; set; } = "Vehicle";

            [Required(ErrorMessage = "Please select the vehicle or entity.")]
            [Display(Name = "Issued To")]
            public int issued_to_id { get; set; }

            [Required(ErrorMessage = "Please enter the odometer reading.")]
            [Display(Name = "Odometer Reading")]
            public decimal odometer_reading { get; set; }

            [Required(ErrorMessage = "Please enter the fuel quantity.")]
            [Display(Name = "Quantity (Litres)")]
            public int quantity { get; set; }

            [Display(Name = "Balance After")]
            public decimal balance_after { get; set; }
            public decimal available_qty { get; set; }
        }



        [BindProperty(SupportsGet = true)]
        public IssueFuel issueFuel { get; set; }

        public issue_fuelModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task OnGet(int id)
        {

            await viewTanks();
            await viewVehicles();
            if (id == 0)
            {
                issueFuel = new IssueFuel();

            }
            else if (id > 0)
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
                var userdetailsJson = HttpContext.Session.GetString("userdetails");
                var userid = 0;
                if (!string.IsNullOrEmpty(userdetailsJson))
                {
                    var userobj = JsonSerializer.Deserialize<UserDetails>(userdetailsJson);
                    userid = userobj.Id;
                }

                var fuelrequest = new {
                    id = issueFuel.id,
                    tank_id = issueFuel.tank_id,
                    issuer_id = userid,
                    issue_date = issueFuel.issue_date,
                    fuel_type = issueFuel.fuel_type,
                    issued_to_id = issueFuel.issued_to_id,
                    issued_to_type = "Vehicle",
                    odometer_reading = issueFuel.odometer_reading,
                    quantity = issueFuel.quantity,
                    balance_after = issueFuel.available_qty - issueFuel.quantity 
                };
                var client = _httpClientFactory.CreateClient("API");

                var request = new DBconnect
                {
                    param1 = JsonSerializer.Serialize(fuelrequest),
                    param2 = "",
                    param3 = "",
                    param4 = "",
                    operation = "",
                    search = "",
                    screen = "FuelTypes"
                };

                HttpResponseMessage response;
                if (issueFuel.id <= 0)
                {
                    response = await client.PostAsJsonAsync("api/Fuel/issue-fuel", request);
                }
                else
                {
                    response = await client.PutAsJsonAsync($"api/Fuel/update-issue-fuel/{issueFuel.id}", request);
                }

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var response_ = JsonSerializer.Deserialize<List<DbResult>>(data);

                    var dbResult = response_?.FirstOrDefault()?.DBresult;
                    if (dbResult > 0)
                    {
                        return RedirectToPage("./Index");
                    }
                    else if (dbResult == -1)
                    {
                        ErrorMessage = $"Fuel Already Issued with Odometer Reading : {issueFuel.odometer_reading} to This Vehicle";
                        return Page();
                    }
                }
            }
            return RedirectToPage();
        }

        public async Task OnGetEdit(int id)
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.GetAsync($"api/Fuel/view-issue-fuel-detail/{id}");

            if (response.IsSuccessStatusCode)
            {
                var dbresult = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<IssueFuel>>(dbresult);

                issueFuel = result?.FirstOrDefault();
            }
        }


        public async Task<IActionResult> viewTanks()
        {
            var client = _httpClientFactory.CreateClient("API");
            HttpResponseMessage response = await client.GetAsync("api/Tanks/viewall");

            if (response.IsSuccessStatusCode)
            {
                var dbresult = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(dbresult);
                tanks = data;
            }
            return Page();
        }

        public async Task<IActionResult> viewVehicles()
        {
            var client = _httpClientFactory.CreateClient("API");
            HttpResponseMessage response = await client.GetAsync("api/Vehicle/view-all-vehicles");

            if (response.IsSuccessStatusCode)
            {
                var dbresult = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(dbresult);
                vehicles = data;
            }
            return Page();
        }
    }
}
