// Fluxo de trabalho:
// 1. Configração do  Ambiente:
//  1.1. Inicialização do projeto
//  1.2. Instalação de pacotes (mongodb.driver)
//  1.3. Configuração da string de conexão
//  1.4. Configração da Injeção de Dependência
// 2. Criação da(s) Model(s)
// 4. Criação do Controller: Lógica para listar, criar e alternar o alarme.
// 5. Criação das Views: Interfaces para a tabela (Home) e o cadastro (Novo).

using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1.4. Configração da Injeção de Dependência
// Registra o cliente do MongoDB como um serviço Singleton (uma única instância para a aplicação)
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
