using Microsoft.AspNetCore.Identity;
using VasosInteligentes.Data;
using VasosInteligentes.Models;
using VasosInteligentes.Seeds;
using VasosInteligentes.Services;
using VasosInteligentes.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Conexão com o mongo
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoConnection")
);
builder.Services.AddSingleton<ContextMongoDb>();

//configuração do identity 
var mongoSettings = builder.Configuration.GetSection("MongoConnection").Get<MongoSettings>();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>
    (options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;

    })
    .AddMongoDbStores<ApplicationUser, ApplicationRole, string>(mongoSettings.ConnectionString, mongoSettings.Database)
    .AddDefaultTokenProviders();

// Importando para Scaffolding e as RazorPages
builder.Services.AddRazorPages();

// Configuração envio de email 
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettigs"));
builder.Services.AddSingleton<EmailService>();

var app = builder.Build();

// seeds 
using(var Scope = app.Services.CreateScope())
{
    var services = Scope.ServiceProvider;
    try
    {
        await IdentitySeeds.SeedRolesAndUser(services, "Admin@123");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro seed: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

// Acrescentar app.UseAuthentication. Tem que ser antes do app.UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();


// comentario teste 