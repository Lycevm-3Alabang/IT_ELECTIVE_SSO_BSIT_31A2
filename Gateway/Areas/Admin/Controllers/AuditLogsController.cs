using Gateway.Models;
using ITELECTIVE_SSO.Data;
using ITElectiveSSO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuditLogsController : Controller
    {
        private readonly SsoDbContext _context;
        private const int PageSize = 20;

        public AuditLogsController(SsoDbContext context)
        {
            _context = context;
        }

        // GET /Admin/AuditLogs
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate, int pageIndex = 1)
        {
            var query = _context.AuditLogs.AsQueryable();

            // TODO (Task 6): date filtering + pagination

            return View(new PaginatedList<AuditLog>(new List<AuditLog>(), 0, 1, PageSize));
        }
    }
}