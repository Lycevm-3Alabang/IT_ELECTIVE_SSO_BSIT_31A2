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

        // TASKS 4-9
    }
}