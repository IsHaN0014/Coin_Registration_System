using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Egov.Data;
using Egov.Models;

namespace Egov.Controllers
{
    public class SelectedCoinViewModelController : Controller
    {
        private readonly EgovContext _context;

        public SelectedCoinViewModelController(EgovContext context)
        {
            _context = context;
        }

        // GET: SelectedCoinViewModels
        public async Task<IActionResult> Index()
        {
            var egovContext = _context.SelectedCoinViewModel.Include(s => s.Citizen);
            return View(await egovContext.ToListAsync());
        }

        // GET: SelectedCoinViewModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var selectedCoinViewModel = await _context.SelectedCoinViewModel
                .Include(s => s.Citizen)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (selectedCoinViewModel == null)
            {
                return NotFound();
            }

            return View(selectedCoinViewModel);
        }

        // GET: SelectedCoinViewModels/Create
        public IActionResult Create()
        {
            ViewData["CitizenId"] = new SelectList(_context.Citizen, "Id", "Id");
            return View();
        }

        // POST: SelectedCoinViewModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CoinName,Quantity,TotalPrice,CitizenId")] SelectedCoinViewModel selectedCoinViewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(selectedCoinViewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CitizenId"] = new SelectList(_context.Citizen, "Id", "Id", selectedCoinViewModel.CitizenId);
            return View(selectedCoinViewModel);
        }

        // GET: SelectedCoinViewModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var selectedCoinViewModel = await _context.SelectedCoinViewModel.FindAsync(id);
            if (selectedCoinViewModel == null)
            {
                return NotFound();
            }
            ViewData["CitizenId"] = new SelectList(_context.Citizen, "Id", "Id", selectedCoinViewModel.CitizenId);
            return View(selectedCoinViewModel);
        }

        // POST: SelectedCoinViewModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CoinName,Quantity,TotalPrice,CitizenId")] SelectedCoinViewModel selectedCoinViewModel)
        {
            if (id != selectedCoinViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(selectedCoinViewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SelectedCoinViewModelExists(selectedCoinViewModel.Id))
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
            ViewData["CitizenId"] = new SelectList(_context.Citizen, "Id", "Id", selectedCoinViewModel.CitizenId);
            return View(selectedCoinViewModel);
        }

        // GET: SelectedCoinViewModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var selectedCoinViewModel = await _context.SelectedCoinViewModel
                .Include(s => s.Citizen)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (selectedCoinViewModel == null)
            {
                return NotFound();
            }

            return View(selectedCoinViewModel);
        }

        // POST: SelectedCoinViewModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var selectedCoinViewModel = await _context.SelectedCoinViewModel.FindAsync(id);
            if (selectedCoinViewModel != null)
            {
                _context.SelectedCoinViewModel.Remove(selectedCoinViewModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SelectedCoinViewModelExists(int id)
        {
            return _context.SelectedCoinViewModel.Any(e => e.Id == id);
        }
    }
}
