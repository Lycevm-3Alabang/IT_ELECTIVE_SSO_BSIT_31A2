using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        ViewBag.UserId = User.FindFirst("sub")?.Value;

        ViewBag.Email =
            User.FindFirst("email")?.Value ??
            User.FindFirst(ClaimTypes.Email)?.Value;

        ViewBag.Groups = User.FindAll("groups")
            .Select(c => c.Value)
            .ToList();

        ViewBag.Levels = User.FindAll("levels")
            .Select(c => c.Value)
            .ToList();

        return View();
    }
}