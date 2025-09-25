using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FuelManagement.Pages.Refill
{
    public class IndexModel : PageModel
    {
        public readonly IHttpClientFactory _httpClientFactory;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        [BindProperty]
        public List<Dictionary<string,object>> lstRefills { get; set; }
        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGet()
        {
            var client = _httpClientFactory.CreateClient("API");
            HttpResponseMessage request = await client.GetAsync("api/Tanks/view-tanks-refill-history");

            if (request.IsSuccessStatusCode)
            {
                var data = await request.Content.ReadAsStringAsync();
                var _lstRefills = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(data);
                if (!string.IsNullOrEmpty(Search))
                {
                    _lstRefills = _lstRefills.Where(
                        x => x["vendor_name"].ToString().Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                             x["invoice_no"].ToString().Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                             x["quantity_added"].ToString().Equals(Search, StringComparison.OrdinalIgnoreCase)
                        ).ToList();
                    PageNumber = 1;
                }

                TotalRecords = _lstRefills.Count();
                TotalPages = (int)Math.Ceiling(decimal.Divide(TotalRecords, PageSize));

                if (PageNumber < 1) PageNumber = 1;
                if (PageNumber > TotalPages) PageNumber = TotalPages;

                lstRefills = _lstRefills.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

                return Page();
            }
            return Page();
        }
    }
}
