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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Egov.Controllers
{
    public class AdminController : Controller
    {
        private readonly EgovContext _context;

        public AdminController(EgovContext context)
        {
            _context = context;
        }

        [AllowAnonymous]// Allows access without loging in.
        // GET: User/Login
        public IActionResult Login()
        {
            ViewData["ShowLoginRegister"] = true;
            return View();
        }

        // POST: User/Login
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]

        public async Task<IActionResult> Login([Bind("Email,Password")] LoginViewModel loginUser)
        {
            if (ModelState.IsValid)
            {
              
                var validAdmin = (from u in _context.Admin where u.Email == loginUser.Email && u.Password == loginUser.Password && u.Status == true select u).ToList();
                if (validAdmin.Count > 0)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, loginUser.Email),
                new Claim(ClaimTypes.Role, validAdmin[0].Role) // Role-based authentication
            };

                    // Create the identity and add the claims
                    var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimPrincipal = new ClaimsPrincipal(claimIdentity);

                    // Sign the user in
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal);

                    // Redirect based on role
                    if (validAdmin[0].Role == "Developer")
                    {
                        // If the user is a developer, redirect to the Developer dashboard
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        // For low-level admins, redirect to the Admin index page
                        return RedirectToAction("Index", "Citizen");
                    }
                }
                else
                {
                    // If no valid admin is found, show an error message
                    ViewData["Message"] = "Invalid Credentials";
                }
            }
            ViewData["ShowLoginRegister"] = true;
            // Return the same login view if model state is not valid or credentials are invalid
            return View(loginUser);
        }

        [AllowAnonymous]
        // GET: User/Register
        public IActionResult Register()
        {
            ViewData["ShowLoginRegister"] = true;
            return View();
        }

        // POST: User/Register
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]

        public async Task<IActionResult> Register([Bind("Name,Email,Password,ConfirmPassword,PhoneNumber")] RegisterViewModel registerAdmin)
        {
            if (ModelState.IsValid)
            {
                var validAdmin = (from u in _context.Admin where u.Email == registerAdmin.Email select u).ToList();
                if (validAdmin.Count > 0)
                {
                    ViewData["Message"] = "User already exists. Please choose another user";
                }
                else
                {
                    Admin admin = new Admin();
                    admin.Name = registerAdmin.Name;
                    admin.Email = registerAdmin.Email;
                    admin.Password = registerAdmin.Password;
                    admin.PhoneNumber = registerAdmin.PhoneNumber;
                    admin.Role = "Normal";
                    admin.Status = false;

                    _context.Add(admin);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction(nameof(Index));
                    ViewData["Message"] = "Registered Successfully";
                }
            }
            ViewData["ShowLoginRegister"] = true;
            return View(registerAdmin);
        }

        [Authorize(Roles ="Developer")]
        // GET: Admin
        public async Task<IActionResult> Index()
        {
            ViewData["ShowLoginRegister"] = true;

            return View(await _context.Admin.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            ViewData["ShowLoginRegister"] = true;
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Name,Email,Password,PhoneNumber,Role,Status")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ShowLoginRegister"] = true;
            return View(admin);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Name,Email,Password,PhoneNumber,Role,Status")] Admin admin)
        {
            if (id != admin.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.id))
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
            return View(admin);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admin = await _context.Admin.FindAsync(id);
            if (admin != null)
            {
                _context.Admin.Remove(admin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
            return _context.Admin.Any(e => e.id == id);
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Admin");
        }
    }
}
