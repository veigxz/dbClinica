using appReversotask.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace appReversotask.Controllers
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
            // Verifica se o paciente já está autenticado
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
            // Verifica se os dados informados são válidos
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Busca o paciente pelo CPF informado
            var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.Cpf == model.Cpf);

            // Caso o CPF não seja encontrado
            if (paciente == null)
            {
                ModelState.AddModelError(
                    "",
                    "CPF não encontrado. Faça seu cadastro primeiro."
                );

                return View(model);
            }

            // Cria os dados de autenticação do paciente
            var claims = new List<Claim>
            {
                // Identifica o paciente através do Código
                new Claim(
                    ClaimTypes.NameIdentifier,
                    paciente.Codigo.ToString()
                ),

                // Nome do paciente
                new Claim(
                    ClaimTypes.Name,
                    paciente.Nome
                ),

                // CPF do paciente
                new Claim(
                    "CPF",
                    paciente.Cpf
                )
            };

            // Cria a identidade utilizando autenticação por Cookies
            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            // Realiza o login do paciente
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );

            // Salva o ID do paciente na Session
            // para ser utilizado posteriormente pela ConsultaController
            HttpContext.Session.SetInt32(
                "PacienteId",
                paciente.Codigo
            );

            // Após o login, direciona o paciente para a lista de consultas
            return RedirectToAction("Index", "Consulta");
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            // Limpa todos os dados armazenados na Session
            HttpContext.Session.Clear();

            // Encerra a autenticação por Cookies
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            // Retorna para a tela de login
            return RedirectToAction("Login");
        }
    }
}
