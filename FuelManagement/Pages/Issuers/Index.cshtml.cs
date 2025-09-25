using System.Text.Json;
using System.Threading.Tasks;
using FuelManagement.Models;
using FuelmanagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FuelManagement.Pages.Issuers
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;

        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }


        [BindProperty(SupportsGet = true)]
        public string? Search {  get; set; }

        [BindProperty]
        public List<Dictionary<string,object>> lstIssuers { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.GetAsync("api/User/issuers");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var _lstissuer = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(data);

                if (!string.IsNullOrEmpty(Search))
                {
                    _lstissuer = _lstissuer.Where( x => x["issuer_name"].ToString().Contains(Search,StringComparison.OrdinalIgnoreCase) 
                                                     || x["contact"].ToString().Contains(Search,StringComparison.OrdinalIgnoreCase)).ToList();
                    PageNumber = 1;
                }

                TotalRecords = _lstissuer.Count();
                TotalPages = (int)Math.Ceiling(decimal.Divide(TotalRecords, PageSize));

                if(PageNumber < 1) PageNumber = 1;
                if(PageNumber > TotalPages) PageNumber = TotalPages;

                //lstIssuers = _lstissuer.Skip(PageNumber).Take(PageSize).ToList();
                lstIssuers = _lstissuer.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
                return Page();
            }

            return Page();
        }


        public async Task<IActionResult> OnGetDelete(int id)
        {

            var client = _httpClientFactory.CreateClient("API");
            var response = await client.DeleteAsync($"api/User/delete-issuer/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var dbresult = JsonSerializer.Deserialize<List<DbResult>>(data);
                return RedirectToPage();
            }

            return RedirectToPage();
        }
    }
}
