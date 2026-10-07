using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using appReversotask.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;



namespace appReversotask.Controllers
{
    // Controller responsável pelo gerenciamento de consultas (CRUD)
    public class ConsultaController : Controller
    {
        // Contexto do banco de dados
        private readonly DbClinicaContext _context;

        // Injeção de dependência do DbContext
        public ConsultaController(DbClinicaContext context)
        {
            _context = context;
        }

        // GET: Consulta
        // Lista as consultas do paciente autenticado
        public async Task<IActionResult> Index(string pesquisa)
        {
            // Obtém o ID do paciente logado através da Session
            var pacienteId = HttpContext.Session.GetInt32("PacienteId");

            // Se não houver paciente autenticado, redireciona para o login
            if (pacienteId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Busca somente as consultas pertencentes ao paciente logado
            var consultas = _context.Consulta
                .Include(c => c.Medico)
                .Include(c => c.Paciente)
                .Where(c => c.PacienteId == pacienteId.Value);

            // Aplica o filtro caso o usuário tenha digitado alguma coisa
            if (!string.IsNullOrWhiteSpace(pesquisa))
            {
                pesquisa = pesquisa.Trim();

                consultas = consultas.Where(c =>
                    c.StatusConsulta.Contains(pesquisa) ||
                    c.Medico.Nome.Contains(pesquisa) ||
                    c.DataHora.ToString().Contains(pesquisa)
                );
            }

            return View(await consultas.ToListAsync());

        }

        // GET: Consulta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consulta = await _context.Consulta
                .Include(c => c.Medico)
                .Include(c => c.Paciente)
                .FirstOrDefaultAsync(m => m.Codigo == id);

            if (consulta == null)
            {
                return NotFound();
            }

            return View(consulta);
        }

        // GET: Consulta/Create
        public IActionResult Create()
        {
            // Lista de médicos disponíveis
            ViewData["MedicoId"] = new SelectList(
                _context.Medicos,
                "Codigo",
                "Nome"
            );

            return View();
        }

        // POST: Consulta/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("DataHora,StatusConsulta,MedicoId")] Consulta consulta)
        {
            // Obtém o ID do paciente autenticado
            var pacienteId = HttpContext.Session.GetInt32("PacienteId");

            // Caso não esteja autenticado, retorna para o login
            if (pacienteId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Vincula automaticamente a consulta ao paciente logado
            consulta.PacienteId = pacienteId.Value;

            if (ModelState.IsValid)
            {
                _context.Add(consulta);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Recarrega a lista de médicos caso exista erro de validação
            ViewData["MedicoId"] = new SelectList(
                _context.Medicos,
                "Codigo",
                "Nome",
                consulta.MedicoId
            );

            return View(consulta);
        }

        // GET: Consulta/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consulta = await _context.Consulta.FindAsync(id);

            if (consulta == null)
            {
                return NotFound();
            }

            ViewData["MedicoId"] = new SelectList(
                _context.Medicos,
                "Codigo",
                "Nome",
                consulta.MedicoId
            );

            ViewData["PacienteId"] = new SelectList(
                _context.Pacientes,
                "Codigo",
                "Nome",
                consulta.PacienteId
            );

            return View(consulta);
        }

        // POST: Consulta/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Codigo,DataHora,StatusConsulta,PacienteId,MedicoId")] Consulta consulta)
        {
            if (id != consulta.Codigo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(consulta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsultaExists(consulta.Codigo))
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

            ViewData["MedicoId"] = new SelectList(
                _context.Medicos,
                "Codigo",
                "Nome",
                consulta.MedicoId
            );

            ViewData["PacienteId"] = new SelectList(
                _context.Pacientes,
                "Codigo",
                "Nome",
                consulta.PacienteId
            );

            return View(consulta);
        }

        // GET: Consulta/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consulta = await _context.Consulta
                .Include(c => c.Medico)
                .Include(c => c.Paciente)
                .FirstOrDefaultAsync(m => m.Codigo == id);

            if (consulta == null)
            {
                return NotFound();
            }

            return View(consulta);
        }

        // POST: Consulta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consulta = await _context.Consulta.FindAsync(id);

            if (consulta != null)
            {
                _context.Consulta.Remove(consulta);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Verifica se a consulta existe
        private bool ConsultaExists(int id)
        {
            return _context.Consulta.Any(e => e.Codigo == id);
        }
    }
}
