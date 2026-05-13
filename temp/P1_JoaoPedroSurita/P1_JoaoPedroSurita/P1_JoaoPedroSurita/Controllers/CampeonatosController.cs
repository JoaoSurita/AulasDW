using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using P1_JoaoPedroSurita.Models;

namespace P1_JoaoPedroSurita.Controllers
{
    public class CampeonatosController : Controller
    {
        private readonly IMongoCollection<Campeonatos> _campeonatos;

        public CampeonatosController(IMongoDatabase database)
        {
            _campeonatos = database.GetCollection<Campeonatos>("Campeonatos");
        }

        // Listagem
        public async Task<IActionResult> Index()
        {
            var listaBdo = await _campeonatos.Find(_ => true).ToListAsync();

            var model = listaBdo.Select(c => new CampeonatosViewModel
            {
                NomeEvento = c.NomeDoEvento,
                Arena = c.Arena,
                DataEvento = c.DataEvento,
                LimiteParticipantes = c.LimiteParticipantes,
                QuantidadeRobos = c.Robos.Count  
            }).ToList();

            return View(model);
        }

        // Cadastro (GET)
        //public IActionResult Create() = View();

        // Cadastro (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Campeonatos novoCampeonato)
        {
            novoCampeonato.DataEvento = DateTime.Now;
            await _campeonatos.InsertOneAsync(novoCampeonato);
            return RedirectToAction(nameof(Index));
        }

        // Alterar (GET)
        public async Task<IActionResult> Alterar(string id)
        {
            var campeonato = await _campeonatos.Find(c => c.Id == id).FirstOrDefaultAsync();
            return View(campeonato);
        }

        // Alterar (POST) - Replace
        [HttpPost]
        public async Task<IActionResult> Alterar(string id, string arena)
        {
            var campeonato = await _campeonatos.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (campeonato == null)
            {
                campeonato.Arena.Add(new Campeonatos
                {
                    Id = id,
                    Arena = arena,
                    DataEvento = DateTime.Now
                });
                await _campeonatos.ReplaceOneAsync(c => c.Id == id, campeonato);
            }
            return RedirectToAction(nameof(Index));
        }

        // Excluir
        public async Task<IActionResult> Delete(string id)
        {
            await _campeonatos.DeleteOneAsync(c => c.Id == id);
            return RedirectToAction(nameof(Index));
        }
    }
}
