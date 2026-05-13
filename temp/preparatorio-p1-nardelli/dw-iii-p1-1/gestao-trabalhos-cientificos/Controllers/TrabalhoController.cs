using gestao_trabalhos_cientificos.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace gestao_trabalhos_cientificos.Controllers
{
    public class TrabalhoController : Controller
    {
        private readonly IMongoCollection<Trabalho> _trabalhos; // Referência para a coleção de trabalhos

        public TrabalhoController(IMongoDatabase database)
        {
            // Inicializa a coleção "Trabalhos" no MongoDB 
            _trabalhos = database.GetCollection<Trabalho>("Trabalhos");
        }

        // LISTAGEM (HOME)
        public async Task<IActionResult> Index()
        {
            var listaBdo = await _trabalhos.Find(_ => true).ToListAsync(); // Busca todos os documentos

            // Conversão obrigatória para ViewModel usando LINQ
            var model = listaBdo.Select(t => new TrabalhoViewModel
            {
                Id = t.Id!,
                Titulo = t.Titulo,
                AreaTematica = t.AreaTematica,
                QuantidadeAutores = t.Autores.Count, // Conta autores na lista embutida
                MediaNotas = t.Avaliacoes.Any() ? t.Avaliacoes.Average(a => a.Nota) : 0 // Média ou zero 
            }).ToList();

            return View(model); // Envia a lista de ViewModels para a View
        }

        // CADASTRO (GET)
        public IActionResult Create() => View();

        // CADASTRO (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Trabalho novoTrabalho)
        {
            novoTrabalho.DataSubmissao = DateTime.Now; // Define data atual de submissão 
            await _trabalhos.InsertOneAsync(novoTrabalho); // Insere o documento completo com autores 
            return RedirectToAction(nameof(Index));
        }

        // AVALIAR (GET) - Mostra os dados para leitura e o form de nota
        public async Task<IActionResult> Avaliar(string id)
        {
            var trabalho = await _trabalhos.Find(t => t.Id == id).FirstOrDefaultAsync(); // Busca por ID
            return View(trabalho); // Envia o trabalho para exibir Título e Resumo 
        }

        // AVALIAR (POST) - Técnica de Replace 
        [HttpPost]
        public async Task<IActionResult> Avaliar(string id, int nota, string comentario)
        {
            var trabalho = await _trabalhos.Find(t => t.Id == id).FirstOrDefaultAsync(); // Busca o original 
            if (trabalho != null)
            {
                trabalho.Avaliacoes.Add(new Avaliacao
                {
                    Nota = nota,
                    Comentario = comentario,
                    DataAvaliacao = DateTime.Now
                }); // Adiciona nova avaliação no objeto C# 
                await _trabalhos.ReplaceOneAsync(t => t.Id == id, trabalho); // Salva por cima (Replace) 
            }
            return RedirectToAction(nameof(Index));
        }

        // EXCLUIR 
        public async Task<IActionResult> Delete(string id)
        {
            await _trabalhos.DeleteOneAsync(t => t.Id == id); // Remove o documento do banco
            return RedirectToAction(nameof(Index));
        }
    }
}
