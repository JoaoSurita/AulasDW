using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VasosInteligentes.Models;

namespace VasosInteligentes.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsuariosController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = _userManager.Users.ToList();
            return View(usuarios);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser model, string senha)
        {
            if (ModelState.IsValid)
            {
                model.UserName = model.Email;
                var resultado = await _userManager.CreateAsync(model, senha);
                if (resultado.Succeeded) return RedirectToAction(nameof(Index));

                foreach (var erro in resultado.Errors)
                    ModelState.AddModelError("", erro.Description);
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario != null) await _userManager.DeleteAsync(usuario);
            return RedirectToAction(nameof(Index));
        }
    }
}