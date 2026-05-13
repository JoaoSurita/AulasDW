// Fluxo de trabalho
// 1. Criação das Models
// 2. Configuração da conexão
// 3. Controller
// 4. Views

using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 2. Conexão com o banco de dados
// Registra o cliente do MongoDB como Singleton (uma única instância para toda a aplicação)
builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration.GetValue<string>("MongoDbSettings:ConnectionString")));

// Configura a injeção do banco de dados específico para os Controllers
builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>(); // Recupera o cliente configurado acima
    var databaseName = builder.Configuration.GetValue<string>("MongoDbSettings:DatabaseName"); // Nome do banco
    return client.GetDatabase(databaseName); // Retorna a instância do banco de dados
});

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
