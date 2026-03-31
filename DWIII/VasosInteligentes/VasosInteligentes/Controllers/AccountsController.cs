using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VasosInteligentes.Models;

namespace VasosInteligentes.Controllers
{
    public class AccountsController : Controller
    {

        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> SignInManager) 
        { 
            _userManager = userManager;
            _signInManager = SignInManager;
        }


        // GET
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task <IActionResult> Login(
            [Required][EmailAddress] string email,
            [Required] string password)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appuser = await _userManager.FindByNameAsync(email);
                if(appuser == null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result =
                        await _signInManager.PasswordSignInAsync(appuser, password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction ("Index", "Home");
                    }
                    ModelState.AddModelError(nameof(email), "Verifique as credenciais");
                }
            }
            return View();
        } // login
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    } // classe
} // namespace 
