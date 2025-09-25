using System.Text.Json;
using FuelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FuelManagement.Pages.Tanks
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
        public List<Dictionary<string,object>> lstTanks { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public string Search {  get; set; }
        public async Task<IActionResult> OnGet()
        {
            var client = _httpClientFactory.CreateClient("API");
            HttpResponseMessage request = await client.GetAsync("api/Tanks/viewall");

            if (request.IsSuccessStatusCode)
            {
                var data = await request.Content.ReadAsStringAsync();
                var _lsttanks = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(data);
                if (!string.IsNullOrEmpty(Search))
                {
                    _lsttanks = _lsttanks.Where(
                        x => x["code"].ToString().Contains(Search,StringComparison.OrdinalIgnoreCase) || 
                             x["fuel_type"].ToString().Contains(Search,StringComparison.OrdinalIgnoreCase) ||
                             x["tank_size"].ToString().Equals(Search,StringComparison.OrdinalIgnoreCase)
                        ).ToList();
                    PageNumber = 1;
                }

                TotalRecords = _lsttanks.Count();
                TotalPages = (int)Math.Ceiling(decimal.Divide(TotalRecords,PageSize));

                if (PageNumber < 1) PageNumber = 1;
                if (PageNumber > TotalPages) PageNumber = TotalPages;

                lstTanks = _lsttanks.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

                return Page();
            }
            return Page();
        }

        public async Task<IActionResult> OnGetDelete(int id)
        {
            var client = _httpClientFactory.CreateClient("API");
            HttpResponseMessage response = await client.DeleteAsync($"api/tanks/delete/{id}");
            if (response.IsSuccessStatusCode) {
                var data = await response.Content.ReadAsStringAsync();
                var dbresult = JsonSerializer.Deserialize<List<DbResult>>(data);

                if (dbresult?.FirstOrDefault().DBresult > 0)
                {
                    return RedirectToPage();
                }
            }
            return RedirectToPage();
        }
    }
}
