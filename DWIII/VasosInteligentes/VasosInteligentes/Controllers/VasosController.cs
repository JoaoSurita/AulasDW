using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using VasosInteligentes.Data;
using VasosInteligentes.Models;

namespace VasosInteligentes.Controllers
{
    public class VasosController : Controller
    {
        private readonly ContextMongoDb _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VasosController(ContextMongoDb context,
            UserManager<ApplicationUser> userManager)
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
                //criar campos temporários será usado na conversão de Object para string
                new BsonDocument("$addFields", new BsonDocument
                {
                    {"PlantaIdObj", new BsonDocument("$toObjectId", "$PlantaId") }
                }),
                //faz o join usando o campo convertido
                new BsonDocument("$lookup", new BsonDocument
                {
                    {"from", "Planta" },
                    {"localField", "PlantaIdObj" },
                    {"foreignField", "_id" },
                    {"as", "PlantaRelacionada" }
                }),
                //remover campos extras para não "quebrar" o C#
                new BsonDocument("$project", new BsonDocument
                {
                    {"PlantaIdObj", 0 }
                })

            };
            var result = await _context.Vaso.Aggregate<Vaso>(pipeline).ToListAsync();
            return View(result);
        }//método

        [Authorize(Roles = "Usuario")]
        // GET: Vasos
        public async Task<IActionResult> MeusVasos()
        {
            //pegar o usuário logado
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            var vasos = await _context.Vaso
                .Find(v => v.UsuarioId == user.Id).ToListAsync();
            foreach (var vaso in vasos)
            {
                var ultimaLeitura = await _context.LeituraSensor
                    .Find(l => l.VasoId == vaso.Id)
                    .SortByDescending(l => l.DataLeitura)
                    .FirstOrDefaultAsync();
                vaso.UltimaLeitura = ultimaLeitura;
                if (!string.IsNullOrEmpty(vaso.PlantaId))
                {
                    var planta = await _context.Planta
                        .Find(p => p.Id == vaso.PlantaId)
                        .FirstOrDefaultAsync();
                    if (planta != null)
                    {
                        vaso.PlantaRelacionada = new List<Planta> { planta };
                    }
                }
            }
            return View(vasos);
        }//método

        // GET: Vasos/Details/5
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Buscar Vaso

            var vaso = await _context.Vaso.Find(v => v.Id == id).FirstOrDefaultAsync();
            if(vaso == null)
            {
                return NotFound();
            }
            // Buscar os dados da planta
            if(!string.IsNullOrEmpty(vaso.PlantaId))
            {
                var planta = await _context.Planta.Find(p => p.Id == vaso.PlantaId).FirstOrDefaultAsync();
                if(planta != null)
                {
                    vaso.PlantaRelacionada = new List<Planta> { planta };
                }
            }
            // Buscar os dados das leituras(temperatura, unidade,...)
            // Ultimas 24 Horas
            var limiteData = DateTime.UtcNow.AddHours(-24);
            var historicoLeituras = await _context.LeituraSensor.Find(l => l.VasoId == id && l.DataLeitura >= limiteData)
                .SortBy(l=>l.DataLeitura)
                .ToListAsync();
            ViewBag.HistoricoLeituras = historicoLeituras;
            return View(vaso);
        }

        // GET: Vasos/Create
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Create()
        {
            var plantas = await _context.Planta.Find(_ => true).ToListAsync();
            ViewBag.PlantaId = new SelectList(plantas, "Id", "Nome");
            return View();
        }

        // POST: Vasos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Create([Bind("Nome,PlantaId,Localizacao")] Vaso vaso)
        {
            //pegar o usuário logado
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Accounts");
            }
            vaso.UsuarioId = user.Id;
            //como UsuarioId não vem da view, vai criar um erro na
            //ModelState que deve ser retirado
            ModelState.Remove("UsuarioId");
            if (ModelState.IsValid)
            {
                await _context.Vaso.InsertOneAsync(vaso);
                return RedirectToAction(nameof(MeusVasos));
            }
            return View(vaso);
        }

        // GET: Vasos/Edit/5
        [Authorize(Roles = "Usuario")]
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Nome,PlantaId,Localizacao")] Vaso vaso)
        {
            if (id != vaso.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Vaso.ReplaceOneAsync(p => p.Id == id, vaso);
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
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var pipeline = new BsonDocument[]
            {
                //buscar apenas o vaso cujo id vem por parâmetro
                new BsonDocument("$match", new BsonDocument("_id", new BsonObjectId(new ObjectId(id)))),
                //criar campos temporários será usado na conversão de Object para string
                new BsonDocument("$addFields", new BsonDocument
                {
                    {"PlantaIdObj", new BsonDocument("$toObjectId", "$PlantaId") }
                }),
                //faz o join usando o campo convertido
                new BsonDocument("$lookup", new BsonDocument
                {
                    {"from", "Planta" },
                    {"localField", "PlantaIdObj" },
                    {"foreignField", "_id" },
                    {"as", "PlantaRelacionada" }
                }),
                //remover campos extras para não "quebrar" o C#
                new BsonDocument("$project", new BsonDocument
                {
                    {"PlantaIdObj", 0 }
                })

            };
            var vaso = await _context.Vaso.Aggregate<Vaso>(pipeline).FirstOrDefaultAsync();
            if (vaso == null)
            {
                return NotFound();
            }
            return View(vaso);
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
