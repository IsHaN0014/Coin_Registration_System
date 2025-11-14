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
using static Egov.Models.ConfirmOrderViewModel;
using static Egov.Models.SelectedCoinViewModel;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Egov.Models;
using Microsoft.CodeAnalysis.RulesetToEditorconfig;
namespace Egov.Controllers
{
    public class CoinController : Controller
    {
        private readonly EgovContext _context;

        public CoinController(EgovContext context)
        {
            _context = context;


        }

        public IActionResult ConfirmOrder(Dictionary<int, int> quantities)
        {
            var coins = _context.Coin.ToList(); // Fetch all coins from the database

            var selectedCoins = new List<SelectedCoinViewModel>();
            decimal grandTotal = 0;

            foreach (var coin in coins)
            {
                if (quantities.ContainsKey(coin.Id) && quantities[coin.Id] > 0)
                {
                    var quantity = quantities[coin.Id];
                    var total = coin.Price * quantity;
                    grandTotal += total;

                    selectedCoins.Add(new SelectedCoinViewModel
                    {
                        CoinName = coin.Title,
                        Quantity = quantity,
                        TotalPrice = total
                    });
                    
                }
            }

            var model = new ConfirmOrderViewModel
            {
                SelectedCoins = selectedCoins,
                GrandTotal = grandTotal
            };

            return View(model); // Redirecting to the confirmation view
        }


        // GET: Coin
        public async Task<IActionResult> Index()
        {
            return View(await _context.Coin.ToListAsync());
        }

        public async Task<IActionResult> CoinDashboard()
        {
            var coins = await _context.Coin.ToListAsync();
            return View(coins); // Pass the list of coins to the CoinDashboard view
        }

        // GET: Coin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coin = await _context.Coin
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coin == null)
            {
                return NotFound();
            }

            return View(coin);
        }



        // GET: Coin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Weight,Price,Producticon")] Coin coin, IFormFile Producticon)
        {
            if (ModelState.IsValid)
            {
                Guid guid = Guid.NewGuid();
                string fileEx = Producticon.FileName.Substring(Producticon.FileName.LastIndexOf('.'));
                string path = Environment.CurrentDirectory + "/wwwroot/ProductImages/" + guid + fileEx;
                FileStream fileStream = new FileStream(path, FileMode.Create);
                await Producticon.CopyToAsync(fileStream);
                coin.Producticon = guid + fileEx;
                _context.Add(coin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coin);
        }

        // GET: Coin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coin = await _context.Coin.FindAsync(id);
            if (coin == null)
            {
                return NotFound();
            }
            return View(coin);
        }

        // POST: Coin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Weight,Price,Producticon")] Coin coin)
        {
            if (id != coin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoinExists(coin.Id))
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
            return View(coin);
        }

        // GET: Coin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coin = await _context.Coin
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coin == null)
            {
                return NotFound();
            }

            return View(coin);
        }

        // POST: Coin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coin = await _context.Coin.FindAsync(id);
            if (coin != null)
            {
                _context.Coin.Remove(coin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoinExists(int id)
        {
            return _context.Coin.Any(e => e.Id == id);
        }
    }

}