using catalogo_rastreamento_lixo_espacial.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace catalogo_rastreamento_lixo_espacial.Controllers
{
    public class ObjetoEspacialController : Controller
    {
        private readonly IMongoCollection<ObjetoEspacial> _objetos;

        public ObjetoEspacialController(IMongoDatabase database)
        {
             _objetos = database.GetCollection<ObjetoEspacial>("ObjetosEspaciais"); 
    }

        // LISTAGEM 
        public async Task<IActionResult> Index()
        {
            var listaBdo = await _objetos.Find(_ => true).ToListAsync();

            // Conversão para ViewModel usando LINQ para extrair a última altitude 
            var model = listaBdo.Select(o => new ObjetoEspacialViewModel
            {
                Id = o.Id!,
                Nome = o.Nome,
                NoradId = o.NoradId.ToString(),
                TipoObjeto = o.TipoObjeto,
                UltimaAltitude = o.HistoricoOrbital.Any() ? o.HistoricoOrbital.Last().Altitude : 0
            }).ToList();

             return View(model); 
    }

        // CADASTRO
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(ObjetoEspacial novoObjeto)
        {
             await _objetos.InsertOneAsync(novoObjeto);
        return RedirectToAction(nameof(Index));
        }

        // TELEMETRIA (GET) 
        public async Task<IActionResult> AtualizarTelemetria(string id)
        {
             var objeto = await _objetos.Find(o => o.Id == id).FirstOrDefaultAsync();
         return View(objeto);
    }

        // TELEMETRIA (POST) - Técnica de Replace
        [HttpPost]
        public async Task<IActionResult> AtualizarTelemetria(string id, double altitude, double velocidade, DateTime dataLeitura)
        {
             var objeto = await _objetos.Find(o => o.Id == id).FirstOrDefaultAsync();
        if (objeto != null)
            {
                objeto.HistoricoOrbital.Add(new HistoricoOrbital
                {
                    Altitude = altitude,
                    Velocidade = velocidade,
                    DataLeitura = dataLeitura
                
                });
             await _objetos.ReplaceOneAsync(o => o.Id == id, objeto);
        }
            return RedirectToAction(nameof(Index));
        }

        // REMOVER 
        public async Task<IActionResult> Delete(string id)
        {
            await _objetos.DeleteOneAsync(o => o.Id == id);
            return RedirectToAction(nameof(Index));
        }
    }
}
