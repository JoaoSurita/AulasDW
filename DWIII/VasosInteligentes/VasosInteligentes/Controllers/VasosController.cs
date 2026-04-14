using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        public VasosController(ContextMongoDb context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Vasos
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {

            var pipeline = new BsonDocument[] {
            // criar campos temporarios sera usado na conversao de object para string
            new BsonDocument("$addFields", new BsonDocument
            {
                {"PlantaIdObj", new BsonDocument("$toObjectId", "$PlantaId") }
            }),

            // faz o join usando campo convertido
            new BsonDocument("$lookup", new  BsonDocument
            {
                {"from", "Planta" },
                {"localField", "PlantaIdObj" },
                {"foreignField", "_id" },
                {"as", "PlantaList" }
            }),

            // remover campos extras para nao "quebrar" o c#
            new BsonDocument("$project", new BsonDocument
            {
                {"PlantaIdObj", 0 },
            })
            };

            var result = await _context.Vaso.Aggregate<Vaso>(pipeline).ToListAsync();
            return View(result);

        } // metodo


        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> MeusVasos()
        {

            var pipeline = new BsonDocument[] {
            // criar campos temporarios sera usado na conversao de object para string
            new BsonDocument("$addFields", new BsonDocument
            {
                {"PlantaIdObj", new BsonDocument("$toObjectId", "$PlantaId") }
            }),

            // faz o join usando campo convertido
            new BsonDocument("$lookup", new  BsonDocument
            {
                {"from", "Planta" },
                {"localField", "PlantaIdObj" },
                {"foreignField", "_id" },
                {"as", "PlantaList" }
            }),

            // remover campos extras para nao "quebrar" o c#
            new BsonDocument("$project", new BsonDocument
            {
                {"PlantaIdObj", 0 },
            })
            };

            var result = await _context.Vaso.Aggregate<Vaso>(pipeline).ToListAsync();
            return View(result);

        } // metodo

        // GET: Vasos/Details/5
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // pegar usuario logado
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            { //                         Metodo, Controller
                return RedirectToAction("Login", "Accounts");
            }

            var usuarioId = user.Id;

            var pipeline = new BsonDocument[] {

                new BsonDocument("$match", new BsonDocument("UserId", usuarioId)),

            // criar campos temporarios sera usado na conversao de object para string
            new BsonDocument("$addFields", new BsonDocument
            {
                {"PlantaIdObj", new BsonDocument("$toObjectId", "$PlantaId") }
            }),

            // faz o join usando campo convertido
            new BsonDocument("$lookup", new  BsonDocument
            {
                {"from", "Planta" },
                {"localField", "PlantaIdObj" },
                {"foreignField", "_id" },
                {"as", "PlantaList" }
            }),

            // remover campos extras para nao "quebrar" o c#
            new BsonDocument("$project", new BsonDocument
            {
                {"PlantaIdObj", 0 },
            })
            };

            var vaso = await _context.Vaso.Aggregate<Vaso>(pipeline).FirstOrDefaultAsync();
            return View(vaso);

        }

        [Authorize(Roles = "Usuario")]
        // GET: Vasos/Create
        public async Task<IActionResult> Create()
        {
            // pegou todas as plantas
            var plantas = await _context.Planta.Find(_ => true).ToListAsync();
            // colocou apenas id e nome na viewbag para uso na view para carregar um select option
            ViewBag.PlantaId = new SelectList(plantas, "Id", "Nome");
            return View();
        }

        // POST: Vasos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,PlantaId,Localizacao")] Vaso vaso)
        {
            // pegar usuario logado
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            { //                         Metodo, Controller
                return RedirectToAction("Login", "Accounts");
            }

            vaso.UsuarioId = user.Id;
            // como UsuarioId nao vem da view, vai criar um erro ModelState que deve ser retirado
            ModelState.Remove("UsuarioId");

            if (ModelState.IsValid)
            {
                await _context.Vaso.InsertOneAsync(vaso);
                return RedirectToAction(nameof(Index));
            }
            return View(vaso);
        }

        // GET: Vasos/Edit/5
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Edit(string id)
        {
            // procura se o id do vaso existe
            if (id == null)
            {
                return NotFound();
            }

            // guarda o vaso
            var vaso = await _context.Vaso.Find(m => m.Id == id).FirstOrDefaultAsync();

            // valida
            if (vaso == null)
            {
                return NotFound();
            }

            // coloca em uma viewbag
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
            // se o id passado por parametro e o mesmo do obj
            if (id != vaso.Id)
            {
                return NotFound();
            }

            // verifica se e valido
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
                return RedirectToAction(nameof(Index));
            }
            return View(vaso);
        }

        // GET: Vasos/Delete/5
        [Authorize(Roles = "Usuario")]
        public async Task<IActionResult> Delete(string id)
        {
            var pipeline = new BsonDocument[] {

                // buscar apenas o vaso cujo id foi passado por parametro

                new BsonDocument("$match", new BsonDocument("_id", new BsonObjectId(new ObjectId(id)))),

            // criar campos temporarios sera usado na conversao de object para string
            new BsonDocument("$addFields", new BsonDocument
            {
                {"PlantaIdObj", new BsonDocument("$toObjectId", "$PlantaId") }
            }),

            // faz o join usando campo convertido
            new BsonDocument("$lookup", new  BsonDocument
            {
                {"from", "Planta" },
                {"localField", "PlantaIdObj" },
                {"foreignField", "_id" },
                {"as", "PlantaList" }
            }),

            // remover campos extras para nao "quebrar" o c#
            new BsonDocument("$project", new BsonDocument
            {
                {"PlantaIdObj", 0 },
            })
            };

            var vaso = await _context.Vaso.Aggregate<Vaso>(pipeline).FirstOrDefaultAsync();
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
            return RedirectToAction(nameof(Index));

        }



        private async Task<bool> VasoExists(string id)
        {
            return await _context.Vaso.Find(e => e.Id == id).AnyAsync();
        }

    }
}
