using Gateway.Areas.Admin.Models;
using ITELECTIVE_SSO.Data;
using ITElectiveSSO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GroupsController : Controller
    {
        private readonly SsoDbContext _context;

        public GroupsController(SsoDbContext context)
        {
            _context = context;
        }

        // GET /Admin/Groups
        public async Task<IActionResult> Index()
        {
            var groups = await _context.Groups
                .Include(g => g.TenantApp)
                .OrderBy(g => g.TenantApp.Name)
                .ThenBy(g => g.PowerLevel)
                .ToListAsync();

            return View(groups);
        }

        // GET /Admin/Groups/Create
        public async Task<IActionResult> Create()
        {
            var model = new GroupViewModel
            {
                TenantApps = await GetTenantAppOptions()
            };

            return View(model);
        }

        private async Task<List<SelectListItem>> GetTenantAppOptions()
        {
            return await _context.TenantApps
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                })
                .ToListAsync();
        }

        // TASK 4 

        // TASK 5

        // TASK 6

        // TASK 7

        private static string BuildPrefixedName(string appName, string groupName)
        {
            var baseName = StripPrefix(groupName, appName);
            return $"{appName}-{baseName}";
        }

        private static string StripPrefix(string name, string appName)
        {
            var prefix = $"{appName}-";
            return name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                ? name[prefix.Length..]
                : name;
        }
    }
}