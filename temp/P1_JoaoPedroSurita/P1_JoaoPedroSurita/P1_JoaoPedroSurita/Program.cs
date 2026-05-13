using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration.GetValue<string>("MongoDbSettings:ConnectionString")));

// Permite injetar o banco de dados específico diretamente nos serviços ou controllers
builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>(); // Recupera o cliente configurado acima
    var databaseName = builder.Configuration.GetValue<string>("MongoDbSettings:DatabaseName"); // Pega o nome do banco
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
