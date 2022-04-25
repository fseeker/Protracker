using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using demo_7.Models;

namespace demo_7.Controllers
{
    public class UpdateProjectController : Controller
    {
        private readonly ProTrackerDbContext _context;

        public UpdateProjectController(ProTrackerDbContext context)
        {
            _context = context;
        }

        // GET: UpdateProject
        public async Task<IActionResult> Index()
        {
            return View(await _context.UpdateProjects.ToListAsync());
        }

        // GET: UpdateProject/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var updateProject = await _context.UpdateProjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (updateProject == null)
            {
                return NotFound();
            }

            return View(updateProject);
        }

        // GET: UpdateProject/Create
        public IActionResult Create(int? id)
        {
            ViewBag.Pid = id;
            return View();
        }

        // POST: UpdateProject/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Version,PushDate,Description,ProjectId")] UpdateProject updateProject)
        {
            if (ModelState.IsValid)
            {
                _context.Add(updateProject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(updateProject);
        }

        // GET: UpdateProject/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var updateProject = await _context.UpdateProjects.FindAsync(id);
            if (updateProject == null)
            {
                return NotFound();
            }
            return View(updateProject);
        }

        // POST: UpdateProject/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Version,PushDate,Description,ProjectId")] UpdateProject updateProject)
        {
            if (id != updateProject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(updateProject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UpdateProjectExists(updateProject.Id))
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
            return View(updateProject);
        }

        // GET: UpdateProject/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var updateProject = await _context.UpdateProjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (updateProject == null)
            {
                return NotFound();
            }

            return View(updateProject);
        }

        // POST: UpdateProject/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var updateProject = await _context.UpdateProjects.FindAsync(id);
            _context.UpdateProjects.Remove(updateProject);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UpdateProjectExists(int id)
        {
            return _context.UpdateProjects.Any(e => e.Id == id);
        }
    }
}
