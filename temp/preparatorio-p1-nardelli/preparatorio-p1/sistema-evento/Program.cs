// Fluxo de trabalho:
// 1. Configração do  Ambiente:
//  1.1. Inicialização do projeto
//  1.2. Instalação de pacotes
//      1.2.1. Pomelo.EntityFrameworkCore.MySql
//      1.2.2. Microsoft.EntityFrameworkCore.Design
//      1.2.3. Microsoft.EntityFrameworkCore.Tools
// 2. Criação da(s) Model(s)
// 3. Criação do DbContext: A classe que gerencia a conexão e as tabelas do MySQL. 
//  3.1. Criação de uma pasta Data
//  3.2. Criação do AppDbContext.cs
// 4. Configuração da conexão com o banco de dados
//  4.1. Configuração da string de conexão no appsettings.json
//  4.2. Configuração da conexão no program.cs
// 5. Migrations: Comandos para gerar o banco de dados e as tabelas automaticamente.  
//  5.1. Add-Migration Inicial
//  5.1. Update-Database
// 5. CRUD de Evento: Criação das telas, garantindo que apareçam os nomes (Patrocinador e Tipo) em vez dos IDs.


using Microsoft.EntityFrameworkCore;
using sistema_evento.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//  4.2. Configuração da conexão no program.cs
// Recupera a string de conexão definida no appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configura o DbContext para usar a string e detectar a versão do MySQL automaticamente
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
