using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using appReversotask.Models;

namespace appReverso.Controllers
{
    public class AccountController : Controller
    {
        private readonly DbClinicaContext _context;

        public AccountController(DbClinicaContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Consulta");
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Busca o paciente pelo CPF digitado
            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.Cpf == model.Cpf);

            if (paciente == null)
            {
                ModelState.AddModelError("", "CPF não encontrado. Faça seu cadastro primeiro.");
                return View(model);
            }

            // Criando os dados da sessão (Claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, paciente.Codigo.ToString()),
                new Claim(ClaimTypes.Name, paciente.Nome),
                new Claim("CPF", paciente.Cpf)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Consulta");
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}