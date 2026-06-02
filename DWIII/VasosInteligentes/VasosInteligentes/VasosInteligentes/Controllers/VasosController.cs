using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VasosInteligentes.Data;
using VasosInteligentes.Models;

namespace VasosInteligentes.Controllers
{
    public class VasosController : Controller
    {
        private readonly ContextMongoDb _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public VasosController(ContextMongoDb context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Administrador")]
        // GET: Vasos
        public async Task<IActionResult> Index()
        {
            var pipeline = new BsonDocument[]
            {
                // Cria campos temporários que serão utilizados na conversão de object para string
                new BsonDocument("$addFields", new BsonDocument
                {
                    { "PlantaIdObj", new BsonDocument("$toObjectId", "$PlantaId") }
                }),

                // Faz o "join" usando o campo convertido
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Planta" },
                    { "localField", "PlantaIdObj" },
                    { "foreignField", "_id" },
                    { "as", "PlantaRelacionada" }
                }),

                // Remove os camposs extras para não quebrar o C#
                new BsonDocument("$project", new BsonDocument
                {
                    { "PlantaIdObj", 0 }
                })
            };
            var result = await _context.Vaso.Aggregate<Vaso>(pipeline).ToListAsync();
            return View(result);
        }

        [Authorize(Roles = "Usuario")]
        // GET: Vasos
        public async Task<IActionResult> MeusVasos()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            var usuarioId = user.Id;

            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("UsuarioId", usuarioId)),
                // Cria campos temporários que serão utilizados na conversão de object para string
                new BsonDocument("$addFields", new BsonDocument
                {
                    { "PlantaIdObj", new BsonDocument("$toObjectId", "$PlantaId") }
                }),

                // Faz o "join" usando o campo convertido
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Planta" },
                    { "localField", "PlantaIdObj" },
                    { "foreignField", "_id" },
                    { "as", "PlantaRelacionada" }
                }),

                // Remove os camposs extras para não quebrar o C#
                new BsonDocument("$project", new BsonDocument
                {
                    { "PlantaIdObj", 0 }
                })
            };
            var result = await _context.Vaso.Aggregate<Vaso>(pipeline).ToListAsync();
            return View(result);
        }

        // Dashboard
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Dashboard()
        {
            // Obtém o usuário logado
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            // Busca todos os vasos que pertencem ao usuário logado
            var vasos = await _context.Vaso
                .Find(v => v.UsuarioId == user.Id)
                .ToListAsync();

            // Listas que vão guardar as médias de cada período para cada vaso.
            var mediasManha = new List<double>();
            var mediasTarde = new List<double>();
            var mediasNoite = new List<double>();
            var nomesVasos = new List<string>();

            // Para cada vaso, busca as leituras e calcula as médias por período
            foreach (var vaso in vasos)
            {
                // Adiciona o nome do vaso à lista de rótulos
                nomesVasos.Add(vaso.Nome ?? "Sem nome");

                // Busca TODAS as leituras do vaso
                var leituras = await _context.LeituraSensor
                    .Find(l => l.VasoId == vaso.Id)
                    .ToListAsync();

                // Cálculo da média de Manhã (06:00 – 11:59)
                var leiturasManha = leituras
                    .Where(l => l.DataLeitura.ToLocalTime().Hour >= 6 &&
                                l.DataLeitura.ToLocalTime().Hour < 12)
                    .ToList();

                mediasManha.Add(leiturasManha.Any()
                    ? Math.Round(leiturasManha.Average(l => l.Luminosidade), 2)
                    : 0);

                // Cálculo da média da Tarde (12:00 – 17:59)
                var leiturasTarde = leituras
                    .Where(l => l.DataLeitura.ToLocalTime().Hour >= 12 &&
                                l.DataLeitura.ToLocalTime().Hour < 18)
                    .ToList();

                mediasTarde.Add(leiturasTarde.Any()
                    ? Math.Round(leiturasTarde.Average(l => l.Luminosidade), 2)
                    : 0);

                // Cálculo da média da Noite (18:00 – 00:00)
                var leiturasNoite = leituras
                    .Where(l => l.DataLeitura.ToLocalTime().Hour >= 18)
                    .ToList();

                mediasNoite.Add(leiturasNoite.Any()
                    ? Math.Round(leiturasNoite.Average(l => l.Luminosidade), 2)
                    : 0);
            }

            // Serializa os dados para JSON e envia para a View via ViewBag.
            ViewBag.NomesVasos = System.Text.Json.JsonSerializer.Serialize(nomesVasos);
            ViewBag.MediasManha = System.Text.Json.JsonSerializer.Serialize(mediasManha);
            ViewBag.MediasTarde = System.Text.Json.JsonSerializer.Serialize(mediasTarde);
            ViewBag.MediasNoite = System.Text.Json.JsonSerializer.Serialize(mediasNoite);

            return View();
        }

        // GET: Vasos/Details/5
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var vaso = await _context.Vaso.Find(v => v.Id == id).FirstOrDefaultAsync();
            if (vaso == null)
            {
                return NotFound();
            }

            // Busca os dados da planta que está no vaso
            if (!string.IsNullOrEmpty(vaso.PlantaId))
            {
                var planta = await _context.Planta.Find(p => p.Id == vaso.PlantaId).FirstOrDefaultAsync();
                if (planta != null)
                {
                    vaso.PlantaRelacionada = new List<Planta> { planta };
                }
            }

            // Busca os dados das leituras
            // Limita em 24h 
            var limiteData = DateTime.UtcNow.AddHours(-24);
            var historicoLeituras = await _context.LeituraSensor.Find(l => l.VasoId == id && l.DataLeitura >= limiteData)
                .SortByDescending(l => l.DataLeitura)
                .ToListAsync();
            ViewBag.HistoricoLeituras = historicoLeituras;
            return View(vaso);
        }

        // GET: Vasos/Create
        public async Task<IActionResult> Create()
        {
            var plantas = await _context.Planta.Find(_ => true).ToListAsync();
            ViewBag.PlantaId = new SelectList(plantas, "Id", "Nome");
            return View();
        }

        // POST: Vasos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Create([Bind("Id,Nome,PlantaId,Localizacao")] Vaso vaso)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            var usuarioId = user.Id;
            ModelState.Remove("UsuarioId");

            if (ModelState.IsValid)
            {
                vaso.UsuarioId = usuarioId;
                await _context.Vaso.InsertOneAsync(vaso);
                return RedirectToAction(nameof(MeusVasos));
            }
            return View(vaso);
        }

        // GET: Vasos/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vaso = await _context.Vaso.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (vaso == null)
            {
                return NotFound();
            }
            var plantas = await _context.Planta.Find(_ => true).ToListAsync();
            ViewBag.PlantaId = new SelectList(plantas, "Id", "Nome", vaso.PlantaId);
            return View(vaso);
        }

        // POST: Vasos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Nome,PlantaId,Localizacao,UsuarioId")] Vaso vaso) // ✅ adicionou UsuarioId no Bind
        {
            if (id != vaso.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Vaso.ReplaceOneAsync(p => p.Id == vaso.Id, vaso);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await VasoExists(vaso.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MeusVasos));
            }
            return View(vaso);
        }

        // GET: Vasos/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("_id", new BsonObjectId(new ObjectId(id)))),
                // Cria campos temporários que serão utilizados na conversão de object para string
                new BsonDocument("$addFields", new BsonDocument
                {
                    { "PlantaIdObj", new BsonDocument("$toObjectId", "$PlantaId") }
                }),

                // Faz o "join" usando o campo convertido
                new BsonDocument("$lookup", new BsonDocument
                {
                    { "from", "Planta" },
                    { "localField", "PlantaIdObj" },
                    { "foreignField", "_id" },
                    { "as", "PlantaRelacionada" }
                }),

                // Remove os camposs extras para não quebrar o C#
                new BsonDocument("$project", new BsonDocument
                {
                    { "PlantaIdObj", 0 }
                })
            };
            var result = await _context.Vaso.Aggregate<Vaso>(pipeline).FirstOrDefaultAsync();
            return View(result);
        }

        // POST: Vasos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _context.Vaso.DeleteOneAsync(m => m.Id == id);
            return RedirectToAction(nameof(MeusVasos));
        }

        private async Task<bool> VasoExists(string id)
        {
            return await _context.Vaso.Find(e => e.Id == id).AnyAsync();
        }
    }
}
