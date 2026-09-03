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

        // ADD OTHER ACTIONS HERE (P3-P6)
    }
}