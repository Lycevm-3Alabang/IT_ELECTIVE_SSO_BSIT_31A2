using Microsoft.AspNetCore.Mvc;

namespace SSO_Gateway.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.HasData = false;

            ViewBag.TotalUsers = 0;
            ViewBag.ActiveApps = 0;
            ViewBag.AssignedGroups = 0;
            ViewBag.AuditEvents = 0;

            return View();
        }

        public IActionResult Users() => View();
        public IActionResult Apps() => View();
        public IActionResult Groups() => View();
        public IActionResult AuditLogs() => View();
    }
}
