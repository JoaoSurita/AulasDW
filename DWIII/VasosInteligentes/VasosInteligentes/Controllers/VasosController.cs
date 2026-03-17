using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public VasosController(ContextMongoDb context)
        {
            _context = context;
        }

        // GET: Vasos
        public async Task<IActionResult> Index()
        {
            var pipeline = new BsonDocument[]
            {
                // Criar campos temporários será usado conversão de Object para string
                new BsonDocument("$addFields", new BsonDocument
                {
                    {"PlantaIdObj", new BsonDocument("$toObjectId", "$PlantaId")}
                }),
                // Faz o "JOIN" usando o campo convertido
                new BsonDocument("$lookup", new BsonDocument
                {
                    {"from", "Planta" },
                    {"localField", "PlantaIdObj" },
                    {"foreignField", "_id" },
                    {"as", "PlantaRelacionada" }
                }),
                // Remover campos extras para não "quebrar" o C#
                new BsonDocument("$project", new BsonDocument
                {
                    {"PlantaIdObj", 0 }
                })
            };
            var result = await _context.Vaso.Aggregate<Vaso>(pipeline).ToListAsync();
            return View(result);
        }// Método

        // GET: Vasos/Details/5
        /* public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vaso = await _context.Vasos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vaso == null)
            {
                return NotFound();
            }

            return View(vaso);
        }*/

        // GET: Vasos/Create
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
        public async Task<IActionResult> Create([Bind("Nome,PlantaId,Localizacao")] Vaso vaso)
        {
            if (ModelState.IsValid)
            {
                await _context.Vaso.InsertOneAsync(vaso);
                return RedirectToAction(nameof(Index));
            }
            return View(vaso);
        }

        // GET: Vasos/Edit/5
        /* public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vaso = await _context.Vasos.FindAsync(id);
            if (vaso == null)
            {
                return NotFound();
            }
            return View(vaso);
        }

        // POST: Vasos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    _context.Update(vaso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VasoExists(vaso.Id))
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
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vaso = await _context.Vasos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vaso == null)
            {
                return NotFound();
            }

            return View(vaso);
        }

        // POST: Vasos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var vaso = await _context.Vasos.FindAsync(id);
            if (vaso != null)
            {
                _context.Vasos.Remove(vaso);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VasoExists(string id)
        {
            return _context.Vasos.Any(e => e.Id == id);
        }
        */
    }
}
