using Gateway.Areas.Admin.Models;
using ITELECTIVE_SSO.Data;
using ITElectiveSSO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TenantAppsController : Controller
    {
        private readonly SsoDbContext _context;

        public TenantAppsController(SsoDbContext context)
        {
            _context = context;
        }

        // GET /Admin/TenantApps
        public async Task<IActionResult> Index()
        {
            var apps = await _context.TenantApps
                .OrderBy(a => a.Name)
                .ToListAsync();

            return View(apps);
        }

        // GET /Admin/TenantApps/Create
        public IActionResult Create()
        {
            return View(new TenantAppViewModel());
        }

        // POST /Admin/TenantApps/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TenantAppViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var nameExists = await _context.TenantApps
                .AnyAsync(a => a.Name == model.Name);

            if (nameExists)
            {
                ModelState.AddModelError(nameof(model.Name), "An app with this name already exists.");
                return View(model);
            }

            var app = new TenantApp
            {
                Name = model.Name,
                ReturnUrl = model.ReturnUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.TenantApps.Add(app);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"App '{app.Name}' was registered successfully.";
            return RedirectToAction(nameof(Index));
        }

        // TASKS 5-9
    }
}