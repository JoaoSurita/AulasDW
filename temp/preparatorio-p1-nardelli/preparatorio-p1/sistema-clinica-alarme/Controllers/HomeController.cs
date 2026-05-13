using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using sistema_clinica_alarme.Models;
using System.Diagnostics;

namespace sistema_clinica_alarme.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMongoCollection<Clinica> _clinicas; // Coleção no MongoDB
        public HomeController(IMongoDatabase database)
        {
            // Inicializa a coleção "Clinicas" dentro do banco de dados fornecido
            _clinicas = database.GetCollection<Clinica>("Clinicas");
        }

        // LISTAR: Mostra todas as clínicas na Home
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var lista = await _clinicas.Find(_ => true).ToListAsync(); // Busca todos os documentos sem filtro
            return View(lista); // Envia a lista para a View
        }

        // CRIAR (GET): Abre a interface de cadastro
        [HttpGet]
        public IActionResult NovaClinica() => View();

        // CRIAR (POST): Envia os dados para salva a nova clínica 
        [HttpPost]
        public async Task<IActionResult> NovaClinica(Clinica novaClinica)
        {
            novaClinica.Alarme = false; // Garante que comece desligado conforme o enunciado
            await _clinicas.InsertOneAsync(novaClinica); // Insere o documento no MongoDB
            return RedirectToAction(nameof(Index)); // Redireciona para a lista
        }

        // ALTERNAR ALARME: Troca ligado/desligado
        public async Task<IActionResult> AlternarAlarme(string id)
        {
            var clinica = await _clinicas.Find(c => c.Id == id).FirstOrDefaultAsync(); // Busca a clínica pelo ID
            if (clinica != null)
            {
                clinica.Alarme = !clinica.Alarme; // Inverte o status atual (true vira false e vice-versa)
                await _clinicas.ReplaceOneAsync(c => c.Id == id, clinica); // Atualiza o documento no banco
            }
            return RedirectToAction(nameof(Index)); // Volta para a tabela
        }
    }
}
