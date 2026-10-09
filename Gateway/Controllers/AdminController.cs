using ITELECTIVE_SSO.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SSO_Gateway.Controllers
{
    public class AdminController : Controller
    {
        private readonly SsoDbContext _context;

        public AdminController(SsoDbContext context)
        {
            _context = context;
        }

        // GET /Admin  (dashboard: live summary stats + latest audit events)
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.ActiveApps = await _context.TenantApps.CountAsync(a => a.IsActive);
            ViewBag.AssignedGroups = await _context.UserGroups
                .Select(ug => ug.GroupId)
                .Distinct()
                .CountAsync();
            ViewBag.AuditEvents = await _context.AuditLogs.CountAsync();

            var recentLogs = await _context.AuditLogs
                .Include(a => a.User)
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .ToListAsync();

            ViewBag.RecentLogs = recentLogs;
            ViewBag.HasData = recentLogs.Count > 0;

            return View();
        }

        public IActionResult Users() => RedirectToAction("Index", "Users", new { area = "Admin" });
        public IActionResult Apps() => RedirectToAction("Index", "TenantApps", new { area = "Admin" });
        public IActionResult Groups() => RedirectToAction("Index", "Groups", new { area = "Admin" });
        public IActionResult AuditLogs() => RedirectToAction("Index", "AuditLogs", new { area = "Admin" });
    }
}