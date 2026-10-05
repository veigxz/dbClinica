using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using appReversotask.Models;
using Microsoft.AspNetCore.Authorization;

namespace appReversotask.Controllers
{
    [Authorize]
    public class ConsultaController : Controller
    {
        private readonly DbClinicaContext _context;

        public ConsultaController(DbClinicaContext context)
        {
            _context = context;
        }

        // GET: Consultas
        public async Task<IActionResult> Index()
        {
            // Verifica se a sessão existe
            var pacienteId = HttpContext.Session.GetInt32("PacienteId");

            if (pacienteId == null)
            {
                // Se não estiver logado, redireciona para a tela de login
                return RedirectToAction("Login", "Account");
            }

            // Código normal da Index...
            var consultas = await _context.Consulta
                .Include(c => c.Medico)
                .Include(c => c.Paciente)
                .Where(c => c.PacienteId == pacienteId) // Mostra apenas as consultas do paciente logado!
                .ToListAsync();

            return View(consultas);
        }


        // GET: Consultas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Consulta == null)
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

        // GET: Consultas/Create
        public IActionResult Create()
        {
            ViewData["MedicoId"] = new SelectList(_context.Medicos, "Codigo", "Codigo");
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Codigo", "Codigo");
            return View();
        }

        // POST: Consultas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,DataHora,StatusConsulta,PacienteId,MedicoId")] Consulta consulta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(consulta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedicoId"] = new SelectList(_context.Medicos, "Codigo", "Codigo", consulta.MedicoId);
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Codigo", "Codigo", consulta.PacienteId);
            return View(consulta);
        }

        // GET: Consultas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Consulta == null)
            {
                return NotFound();
            }

            var consulta = await _context.Consulta.FindAsync(id);
            if (consulta == null)
            {
                return NotFound();
            }
            ViewData["MedicoId"] = new SelectList(_context.Medicos, "Codigo", "Codigo", consulta.MedicoId);
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Codigo", "Codigo", consulta.PacienteId);
            return View(consulta);
        }

        // POST: Consultas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Codigo,DataHora,StatusConsulta,PacienteId,MedicoId")] Consulta consulta)
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
            ViewData["MedicoId"] = new SelectList(_context.Medicos, "Codigo", "Codigo", consulta.MedicoId);
            ViewData["PacienteId"] = new SelectList(_context.Pacientes, "Codigo", "Codigo", consulta.PacienteId);
            return View(consulta);
        }

        // GET: Consultas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Consulta == null)
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

        // POST: Consultas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Consulta == null)
            {
                return Problem("Entity set 'DbClinicaContext.Consulta'  is null.");
            }
            var consulta = await _context.Consulta.FindAsync(id);
            if (consulta != null)
            {
                _context.Consulta.Remove(consulta);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConsultaExists(int id)
        {
          return (_context.Consulta?.Any(e => e.Codigo == id)).GetValueOrDefault();
        }
    }
}
