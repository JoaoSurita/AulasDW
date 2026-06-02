using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VasosInteligentes.Models;

namespace VasosInteligentes.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminUsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email
                };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string email)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                user.Email = email;
                user.UserName = email;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("Index");
        }
    }
}
