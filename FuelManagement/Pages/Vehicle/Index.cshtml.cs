using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FuelManagement.Pages.Vehicle
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        [BindProperty]
        public List<Dictionary<string, object>> lstVehicles { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var client = _httpClientFactory.CreateClient("API");
            HttpResponseMessage request = await client.GetAsync("api/Vehicle/view-all-vehicles");

            if (request.IsSuccessStatusCode)
            {
                var data = await request.Content.ReadAsStringAsync();
                var _lsttanks = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(data);
                if (!string.IsNullOrEmpty(Search))
                {
                    _lsttanks = _lsttanks.Where(
                        x => x["driver_name"].ToString().Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                             x["tank_size"].ToString().Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                             x["odometer_reading"].ToString().Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                             x["vehicle_number"].ToString().Contains(Search, StringComparison.OrdinalIgnoreCase)
                        ).ToList();
                    PageNumber = 1;
                }

                TotalRecords = _lsttanks.Count();
                TotalPages = (int)Math.Ceiling(decimal.Divide(TotalRecords, PageSize));

                if (PageNumber < 1) PageNumber = 1;
                if (PageNumber > TotalPages) PageNumber = TotalPages;

                lstVehicles = _lsttanks.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

                return Page();
            }
            return Page();
        }

    }
}
