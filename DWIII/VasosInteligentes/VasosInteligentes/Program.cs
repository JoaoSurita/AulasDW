using Microsoft.AspNetCore.Identity;
using VasosInteligentes.Data;
using VasosInteligentes.Models;
using VasosInteligentes.Seeds;
using VasosInteligentes.Services;
using VasosInteligentes.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//conexão com o mongoDb
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoConnection"));
builder.Services.AddSingleton<ContextMongoDb>();

//configuração do Identity


var mongoSettings = builder.Configuration
    .GetSection("MongoConnection")
    .Get<MongoSettings>();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>
    (options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
    })
    .AddMongoDbStores<ApplicationUser, ApplicationRole, string>
    (mongoSettings.ConnectionString, mongoSettings.Database)
    .AddDefaultTokenProviders();

//importante para Scaffolding  e as RazorPages para o Identity
builder.Services.AddRazorPages();

//configuração envio de email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<EmailService>();
//simulador
builder.Services.AddHostedService<SensorBackgroundService>();

var app = builder.Build();

//seeds
using(var Scope = app.Services.CreateScope())
{
    var services = Scope.ServiceProvider;
    try
    {
        await IdentitySeeds.SeedRolesAndUser(services, "Admin@123");

    }
    catch(Exception ex)
    {
        Console.WriteLine($"Erro Seed: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

//acrescentar app.UseAuthentication() antes do app.UseAuthorization();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
