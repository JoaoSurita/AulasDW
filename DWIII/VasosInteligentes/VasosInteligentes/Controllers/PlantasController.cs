using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using VasosInteligentes.Data;
using VasosInteligentes.Models;

namespace VasosInteligentes.Controllers
{
    public class PlantasController : Controller
    {
        private readonly ContextMongoDb _context;

        public PlantasController(ContextMongoDb context)
        {
            _context = context;
        }
        [Authorize(Roles="Administrador")]
        // GET: Plantas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Planta.Find(_=>true).ToListAsync());
        }

        // GET: Plantas/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planta = await _context.Planta.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (planta == null)
            {
                return NotFound();
            }

            return View(planta);
        }

        // GET: Plantas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Plantas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nome,UmidadeIdealMin,UmidadeIdealMax,LuminosidadeIdeal")] Planta planta)
        {
            if (ModelState.IsValid)
            {
                await _context.Planta.InsertOneAsync(planta);
                return RedirectToAction(nameof(Index));
            }
            return View(planta);
        }

        // GET: Plantas/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planta = await _context.Planta.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (planta == null)
            {
                return NotFound();
            }
            return View(planta);
        }

        // POST: Plantas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Nome,UmidadeIdealMin,UmidadeIdealMax,LuminosidadeIdeal")] Planta planta)
        {
            if (id != planta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.Planta.ReplaceOneAsync(p => p.Id == id, planta);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await PlantaExists(planta.Id))
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
            return View(planta);
        }

        // GET: Plantas/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planta = await _context.Planta.Find(m => m.Id == id).FirstOrDefaultAsync();
            if (planta == null)
            {
                return NotFound();
            }

            return View(planta);
        }

        // POST: Plantas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _context.Planta.DeleteOneAsync(m => m.Id == id);
            if(result == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> PlantaExists(string id)
        {
            return await _context.Planta.Find(e => e.Id == id).AnyAsync();
        }
    }
}
