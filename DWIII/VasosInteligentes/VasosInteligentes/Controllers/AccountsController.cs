using System.ComponentModel.DataAnnotations;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VasosInteligentes.Models;
using VasosInteligentes.Services;
using VasosInteligentes.ViewModel;

namespace VasosInteligentes.Controllers
{
    public class AccountsController : Controller
    {

        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private EmailService _emailService;
        public AccountsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> SignInManager, EmailService emailService)
        { 
            _userManager = userManager;
            _signInManager = SignInManager;
            _emailService = emailService;
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
            Console.WriteLine($"email:{email} - senha: {password}");
            if (ModelState.IsValid)
            {
                ApplicationUser appuser = await _userManager.FindByEmailAsync(email);
                if(appuser != null)
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

        //get 
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Informe o e-mail");
                return View();
            }
            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            if(user == null)
            {
                return RedirectToAction("ForgotPasswordConfirm");
            }
            //preparar o link para o envio do e-mail 
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodeToken = HttpUtility.UrlEncode(token);
            var callbackUrl = Url.Action("ResetPassword", "Accounts", new {userId=user.Id, token = encodeToken}, Request.Scheme);
            // preparar dados do email
            var assunto = "Redefinição de senha";
            var corpo = $"Clique no link para redefinir sua senha: <a href = '{callbackUrl}'>Redefinir senha</a>";
            // Enviar email 
            await _emailService.SendEmailAsync(email, assunto, corpo);
            return RedirectToAction("ForgotPasswordConfirm");
        }
        public IActionResult ForgotPasswordConfirm()
        {
            return View();
        }
        public IActionResult ResetPassword(string token, string userId)
        {
            if(token == null || token == "")
            {
                ModelState.AddModelError("", "Token Invalido");
            }
            var model = new ResetPasswordViewModel
            {
                Token = token,
                UserId = userId
            };

            return View();
        }

        public IActionResult ResetPasswordConfirm()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirm");
            }
            var decodeToken = HttpUtility.UrlDecode(model.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodeToken, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirm");
            }
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }
    } // classe
} // namespace 
