using ITELECTIVE_SSO.Data;
using ITElectiveSSO.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SsoDbContext _context;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            SsoDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;

            var query = _userManager.Users.OrderBy(u => u.Email);

            var totalUsers = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(users);
        }

        // GET: /Admin/Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/Users/Create
        [HttpPost]
        public async Task<IActionResult> Create(
            string Email,
            string Password,
            string ConfirmPassword)
        {
            // P7

            // P6

            var user = new ApplicationUser
            {
                UserName = Email,
                Email = Email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }

        // Part ni Leano

        // POST: /Admin/Users/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = false;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }
}