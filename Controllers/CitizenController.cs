using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Egov.Data;
using Egov.Models;
using Microsoft.AspNetCore.Authorization;

namespace Egov.Controllers
{
    public class CitizenController : Controller
    {
        private readonly EgovContext _context;

        public CitizenController(EgovContext context)
        {
            _context = context;
        }
        [Authorize]
        // GET: Citizen
        public async Task<IActionResult> Index()
        {
            ViewData["ShowLoginRegister"] = true;
            return View(await _context.Citizen.ToListAsync());
        }

        // GET: Citizen/Details/5
        public async Task<IActionResult> Details(int? id) { 
        var citizen = _context.Citizen.Find(id); // Assuming you're fetching a Citizen
        var selectedCoins = _context.SelectedCoinViewModel.Where(c => c.CitizenId == id).ToList(); // Example for SelectedCoins

        var model = new Egov.Models.FinalView
        {
            Citizen = citizen,
            SelectedCoins = selectedCoins
        };

    return View(model); // Pass the FinalView object to the view
}
        [AllowAnonymous]
        // GET: Citizen/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Citizen/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,MiddleName,Phone,Email,CitizenshipNo,CitizenF,CitizenB,SelectedViewModels")] Citizen citizen, IFormFile CitizenF, IFormFile CitizenB)
        {
            if (ModelState.IsValid)
            {
                // Check if Citizenship Number already exists
                var existingCitizen = await _context.Citizen
                    .FirstOrDefaultAsync(c => c.CitizenshipNo == citizen.CitizenshipNo);

                if (existingCitizen != null)
                {
                    // Add an error message if the Citizenship Number already exists
                    ModelState.AddModelError("CitizenshipNo", "A citizen with this Citizenship Number already exists.");
                    return View(citizen);
                }

                // Save Citizenship F and B images
                Guid guid1 = Guid.NewGuid();
                string fileEx1 = CitizenF.FileName.Substring(CitizenF.FileName.LastIndexOf('.'));
                string path1 = Environment.CurrentDirectory + "/wwwroot/CitizenshipImages/" + guid1 + fileEx1;
                FileStream fileStream1 = new FileStream(path1, FileMode.Create);
                await CitizenF.CopyToAsync(fileStream1);
                citizen.CitizenF = guid1 + fileEx1;

                Guid guid2 = Guid.NewGuid();
                string fileEx2 = CitizenB.FileName.Substring(CitizenB.FileName.LastIndexOf('.'));
                string path2 = Environment.CurrentDirectory + "/wwwroot/CitizenshipImages/" + guid2 + fileEx2;
                FileStream fileStream2 = new FileStream(path2, FileMode.Create);
                await CitizenB.CopyToAsync(fileStream2);
                citizen.CitizenB = guid2 + fileEx2;

                // Add the new citizen to the database
                _context.Add(citizen);
                await _context.SaveChangesAsync();
                if (citizen.SelectedCoinViewModels != null)
                {
                    foreach (var coin in citizen.SelectedCoinViewModels)
                    {
                        var selectedCoin = new SelectedCoinViewModel
                        {
                            CoinName = coin.CoinName,
                            Quantity = coin.Quantity,
                            TotalPrice = coin.TotalPrice,
                            CitizenId = citizen.Id  // Link the selected coin to the citizen
                        };

                        _context.Add(selectedCoin);
                    }
                    await _context.SaveChangesAsync();
                }


                TempData["CitizenId"] = citizen.Id;
                return RedirectToAction("CoinDashboard", "Coin");
            }
            return View(citizen);
        }

        // GET: Citizen/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var citizen = await _context.Citizen.FindAsync(id);
            if (citizen == null)
            {
                return NotFound();
            }
            return View(citizen);
        }

        // POST: Citizen/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,MiddleName,Phone,Email,CitizenshipNo,CitizenF,CitizenB,Status")] Citizen citizen)
        {
            if (id != citizen.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(citizen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CitizenExists(citizen.Id))
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
            return View(citizen);
        }

        // GET: Citizen/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var citizen = await _context.Citizen
                .FirstOrDefaultAsync(m => m.Id == id);
            if (citizen == null)
            {
                return NotFound();
            }

            return View(citizen);
        }

        // POST: Citizen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var citizen = await _context.Citizen.FindAsync(id);
            if (citizen != null)
            {
                _context.Citizen.Remove(citizen);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CitizenExists(int id)
        {
            return _context.Citizen.Any(e => e.Id == id);
        }



        public async Task<IActionResult> Welcome()
        {
            var citizen = await _context.Citizen
                .Where(c => c.Status == true)
                .OrderByDescending(c => c.Id) // Assumes the highest Id represents the most recent citizen
                .FirstOrDefaultAsync();

            if (citizen == null)
            {
                return NotFound("No citizen with status true found.");
            }

            return View(citizen);
        }
    }
}
    