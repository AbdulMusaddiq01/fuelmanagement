using FuelManagement.Middlewares;
using FuelManagement.Services;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();


//builder.Services.AddRazorPages();
builder.Services.AddRazorPages(options =>
{
    options.Conventions.ConfigureFilter(new AuthPageFilter());
});

builder.Services.AddTransient<AuthHandler>();
//builder.Services.AddHttpClient("API").AddHttpMessageHandler<AuthHandler>();

builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("http://localhost:5296/");
}).AddHttpMessageHandler<AuthHandler>();

//builder.Services.AddTransient<AuthMiddleware>();
ExcelPackage.License.SetNonCommercialOrganization("Musaddiq LLC");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();
app.UseSession();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

//app.UseMiddleware<AuthMiddleware>();

app.Run();
