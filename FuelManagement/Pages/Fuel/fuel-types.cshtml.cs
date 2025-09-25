using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Xml.Linq;
using FuelmanagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static FuelManagement.Pages.Fuel.fuel_typesModel;

namespace FuelManagement.Pages.Fuel
{
    public class fuel_typesModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public string ErrorMessage { get; set; }


        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        // Pagination
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 5;
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }



        public fuel_typesModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public class DbResult
        {
            public int DBresult { get; set; }
        }

        public bool ShowModal { get; set; } = false;

        [BindProperty]
        public List<Dictionary<string,object>> LstFuelTypes { get; set; }

        public class FuelType
        {
            public int? id { get; set; }
            [Required(ErrorMessage = "Name is required")]
            public string Name { get; set; }
            public string? ColorCode { get; set; }
            public DateTime? CreatedDate { get; set; }
            public DateTime? UpdateDate { get; set; }
        }

        [BindProperty]
        public FuelType fuelType { get; set; }


        public async Task<IActionResult> OnPostSave()
        {
            if (fuelType.Name == "") 
            {
                ShowModal = true;
                return Page();
            }
            else
            {
                var fuelrequest = new { fuelType.id , name = fuelType.Name , color_code = fuelType.ColorCode };
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
                        return RedirectToPage();
                    }
                    else if (dbResult == -1)
                    {
                        ErrorMessage = $"Fuel with {fuelType.Name} already exists!";
                        ShowModal = true;
                        return Page();
                    }
                }
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGet()
        {
            var client = _httpClientFactory.CreateClient("API");

            var request = new DBconnect
            {
                param1 = "",
                param2 = "",
                param3 = "",
                param4 = "",
                operation = "View",
                search = "",
                screen = "FuelTypes"
            };

            var response = await client.PostAsJsonAsync("api/Fuel/fuel-types", request);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var allitems = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(data);
                

                if (!string.IsNullOrEmpty(Search))
                {
                    allitems = allitems.Where(x => x["Name"].ToString().Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
                    PageNumber = 1;
                }

                TotalRecords = allitems.Count();
                TotalPages = (int)Math.Ceiling(decimal.Divide(TotalRecords, PageSize));

                if (PageNumber < 1) PageNumber = 1;
                if (PageNumber > TotalPages) PageNumber = TotalPages;

                LstFuelTypes = allitems.Skip((PageNumber - 1)*PageSize).Take(PageSize).ToList();

                return Page();
            }

            return Page();
        }

        public IActionResult OnPostCancel()
        {
            ModelState.Clear();
            fuelType = new FuelType();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetDelete(int id)
        {
            var client = _httpClientFactory.CreateClient("API");

            var request = new DBconnect
            {
                param1 = "",
                param2 = "",
                param3 = "",
                param4 = "",
                operation = "Delete",
                search = id.ToString(),
                screen = "FuelTypes"
            };

            var response = await client.PostAsJsonAsync("api/Fuel/fuel-types", request);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
            }
                return RedirectToPage();
        }


        //public async Task<IActionResult> OnGetEdit(int id)
        //{
        //    var client = _httpClientFactory.CreateClient("API");
        //    var response = await client.GetAsync($"api/Fuel/ViewFuelType/{id}");
        //    if (response.IsSuccessStatusCode)
        //    {
                
        //        var data = await response.Content.ReadAsStringAsync();
        //        var details = JsonSerializer.Deserialize<List<FuelType>>(data);

        //        fuelType = details?.FirstOrDefault();

        //    }
        //    return RedirectToPage();
        //}
    }
}
