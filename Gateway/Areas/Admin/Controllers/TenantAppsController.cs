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

        // Tasks 2-9

    }
}