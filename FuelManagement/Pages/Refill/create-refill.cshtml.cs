using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;
using FuelManagement.Models;
using FuelmanagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static FuelManagement.Pages.Issuers.create_issuerModel;
using static FuelManagement.Pages.Tanks.create_tankModel;

namespace FuelManagement.Pages.Refill
{
    public class create_refillModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public string ErrorMessage { get; set; }

        [BindProperty]
        public int? tank_size { get; set; }
        [BindProperty]
        public decimal? available_qty { get; set; }
        [BindProperty]
        public string? uom { get; set; }

        public class Refill
        {
            public int? id { get; set; }

            [Required]
            [Display(Name = "Tank")]
            public int? tank_id { get; set; }

            [Required]
            [Display(Name = "Refill date")]
            public DateTime? refill_date { get; set; }

            [Required]
            [Display(Name = "Quantity")]
            public decimal? quantity_added { get; set; }

            [Required]
            [Display(Name = "Vendor name")]
            public string vendor_name { get; set; }

            [Required]
            [Display(Name = "Invoice no")]
            public string invoice_no { get; set; }

            [Required]
            [Display(Name = "Price")]
            public decimal? cost_per_liter {  get; set; }
            public int? tank_size { get; set; }
            public decimal? available_qty { get; set; }
            public string? uom { get; set; }
            public decimal? total {  get; set; }
        }

        [BindProperty]
        public Refill refill { get; set; }
        public List<Dictionary<string, object>> tanks { get; set; } = new();

        public class TankDetails
        {
            public int id { get; set; }
            public string? code { get; set; }
            public string fuel_type { get; set; }
            public int tank_size { get; set; }
            public string uom { get; set; }
            public decimal available_qty { get; set; }
        }

        public create_refillModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task OnGet(int id) 
        {
           await viewTanks();
            if (id == 0)
            {
                refill = new Refill();
            }
            else
            {
                await getRefillDetails(id);
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
                var refilldata = new {id = refill.id, tank_id = refill.tank_id, refill_date  = refill.refill_date , quantity_added = refill.quantity_added , vendor_name = refill.quantity_added , invoice_no = refill.invoice_no , cost_per_liter = refill.cost_per_liter};
                var client = _httpClientFactory.CreateClient("API");

                var request = new DBconnect
                {
                    param1 = JsonSerializer.Serialize(refilldata),
                    param2 = "",
                    param3 = "",
                    param4 = "",
                    search = "",
                    operation = "",
                    screen = "TankRefills",
                };

                HttpResponseMessage response;
                if (refill.id > 0)
                {
                    response = await client.PutAsJsonAsync($"api/Tanks/update-refill/{refill.id}", request);
                }
                else
                {
                    response = await client.PostAsJsonAsync("api/Tanks/refill-tank", request);
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
                }

            }
                return Page();
        }

        public async Task<IActionResult> viewTanks()
        {
            var client = _httpClientFactory.CreateClient("API");
            HttpResponseMessage response = await client.GetAsync("api/Tanks/viewall");

            if (response.IsSuccessStatusCode)
            {
                var dbresult = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<Dictionary<string,object>>>(dbresult);
                tanks = data;
            }
            return Page();
        }

        public async Task<IActionResult> getRefillDetails(int id)
        {
            var client = _httpClientFactory.CreateClient("API");
            HttpResponseMessage response = await client.GetAsync($"api/Tanks/view-tank-refill-history/{id}");

            if (response.IsSuccessStatusCode)
            {
                var dbresult = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<Refill>>(dbresult);
                refill = data?.FirstOrDefault();
                tank_size = data.First().tank_size;
                available_qty = data.First().available_qty;
            }
            return Page();
        }


    }
}
