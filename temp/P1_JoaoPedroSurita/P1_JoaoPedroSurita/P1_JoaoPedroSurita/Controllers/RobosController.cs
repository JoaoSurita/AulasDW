using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using P1_JoaoPedroSurita.Models;

namespace P1_JoaoPedroSurita.Controllers
{
    public class RobosController : Controller
    {
        private readonly IMongoCollection<Robos> _robos;

        public RobosController(IMongoDatabase database)
        {
            _robos = database.GetCollection<Robos>("Robos");
        }

        // Listagem
        public async Task<IActionResult> Index()
        {
            var listaBdo = await _robos.Find(_ => true).ToListAsync();

            var model = listaBdo.Select(r => new Robos
            {
                Nome = r.Nome,
                Categoria = r.Categoria,
                NivelBateria = r.NivelBateria,
                Status = r.Status
            }).ToList();

            return View(model);
        }

        // Cadastro (GET)
        public IActionResult Create() = View();

        // Cadastro (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Robos novoRobo)
        {
            await _robos.InsertOneAsync(novoRobo);
            return RedirectToAction(nameof(Index));
        }

        // Alterar (GET)
        public async Task<IActionResult> Alterar(string id)
        {
            var robo = await _robos.Find(r => r.Id == id).FirstOrDefaultAsync();
            return View(robo);
        }

        // Alterar (POST) - Replace
        [HttpPost]
        public async Task<IActionResult> Alterar(string id, string nome, string categoria, int nivelBateria, string status)
        {
            var robo = await _robos.Find(r => r.Id == id).FirstOrDefaultAsync();
            if (robo == null)
            {
                robo.Add(new Robos
                {
                    Id = id,
                    Nome = nome,
                    Categoria = categoria,
                    NivelBateria = nivelBateria,
                    Status = status
                });
                await _robos.ReplaceOneAsync(r => r.Id == id, robo);
            }
            return RedirectToAction(nameof(Index));
        }

        // Excluir
        public async Task<IActionResult> Delete(string id)
        {
            await _robos.DeleteOneAsync(r => r.Id == id);
            return RedirectToAction(nameof(Index));
        }
    }
}
