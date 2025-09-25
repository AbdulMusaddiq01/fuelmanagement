using FuelManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FuelManagement.Pages.User
{
    public class IndexModel : PageModel
    {
        public readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        [BindProperty]
        public List<Dictionary<string, object>> lstusers { get; set; }



        public async Task<IActionResult> OnGet()
        {
            var client = _httpClientFactory.CreateClient("API");

            var response = await client.GetAsync("api/User/view");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var _lstusers = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(data);

                if (!string.IsNullOrEmpty(Search))
                {
                    _lstusers = _lstusers.Where(x => x["name"].ToString().Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    PageNumber = 1;
                }

                TotalRecords = _lstusers.Count();
                TotalPages = (int)Math.Ceiling(decimal.Divide(TotalRecords, PageSize));

                if (PageNumber < 1) PageNumber = 1;
                if (PageNumber > TotalPages) PageNumber = TotalPages;

                lstusers = _lstusers.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
                return Page();
            }
            else
            {
                lstusers = [];
            }
            return Page();
        }


        public async Task<IActionResult> OnGetDelete(int id)
        {
            var client = _httpClientFactory.CreateClient("API");
            var response = await client.DeleteAsync($"api/User/delete/{id}");

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
