using System.Data;
using System.Threading.RateLimiting;
using ConsultaCEP.Repositories;
using ConsultaCEP.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

string? environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

if (environmentName != "Production")
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
}

builder.Services.AddScoped<ICepRepository, CepRepository>();
builder.Services.AddScoped<ICepService, CepService>();
builder.Services.AddHttpClient<IViaCepService, ViaCepService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ViaCep"]!);
    client.Timeout = TimeSpan.FromSeconds(5);
});

var connectionString = builder.Configuration.GetConnectionString("LocalSQLServer");
builder.Services.AddScoped<IDbConnection>(sp => 
{
    var connection = new SqlConnection(connectionString);
    // connection.Open();
    return connection;
});

builder.Services.AddMemoryCache();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("default", o =>
    {
        o.PermitLimit = 30;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 10;
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
