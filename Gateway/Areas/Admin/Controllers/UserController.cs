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

        // ADD OTHER ACTIONS HERE (P2-P6)
    }
}