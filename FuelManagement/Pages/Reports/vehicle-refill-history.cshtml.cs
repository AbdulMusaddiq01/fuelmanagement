using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text.Json;
using FuelmanagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using static FuelManagement.Pages.Vehicle.create_vehicleModel;

namespace FuelManagement.Pages.Reports
{
    public class vehicle_refill_historyModel : PageModel
    {
        public readonly IHttpClientFactory _httpClientFactory;

        [Required (ErrorMessage = "Select vehicle.")]
        [Display(Name = "Vehicle")]
        [BindProperty]
        public int VehicleId { get; set; }
        public List<Dictionary<string, object>> vehicles { get; set; } = new();
        public List<Dictionary<string, object>> lstVehicleRefill { get; set; } = new();

        public vehicle_refill_historyModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            ExcelPackage.License.SetNonCommercialPersonal("Nextasoft LLC");
        }

      
        public async Task OnGet()
        {
            await viewVehicles();
        }

        public async Task<IActionResult> OnPostViewReport()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else
            {
                var client = _httpClientFactory.CreateClient("API");
                var request = new DBconnect
                {
                    param1 = "",
                    param2 = "",
                    param3 = "",
                    param4 = "",
                    search = VehicleId.ToString(),
                    operation = "",
                    screen = ""
                };
                var response =  await client.PostAsJsonAsync("api/Reports/vehicle-refill-history", request);
                if (response.IsSuccessStatusCode) { 
                    var dbresult = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<List<Dictionary<string,object>>>(dbresult);
                    lstVehicleRefill = data;
                }
            }
            return Page();
        }

        public IActionResult OnPostDownloadExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells["A1"].Value = "Hello";
                worksheet.Cells["B1"].Value = "World";

                package.Save();
            }

            stream.Position = 0; // Reset stream before returning file

            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "MyExcelFile.xlsx"
            );
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
