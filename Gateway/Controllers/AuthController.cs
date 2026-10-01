using Gateway.Models;
using Gateway.Services;
using ITElectiveSSO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Gateway.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private const string InvalidCredentialsMessage = "Invalid email or password.";
        private const string SuspendedMessage = "Account Suspended";
        private const string LockedOutMessage = "Too many failed login attempts. Please try again in 15 minutes.";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IReturnUrlValidator _returnUrlValidator;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IAuditService _auditService;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IReturnUrlValidator returnUrlValidator,
            IJwtTokenService jwtTokenService,
            IAuditService auditService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _returnUrlValidator = returnUrlValidator;
            _jwtTokenService = jwtTokenService;
            _auditService = auditService;
        }

        // ===== TASK 2: GET /Auth/Login?returnUrl=xxx =====
        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl)
        {

            var app = await _returnUrlValidator.ValidateAsync(returnUrl, GetClientIp());

            if (app == null)
            {
                return View("UnapprovedApp");
            }
            return View(new LoginViewModel { ReturnUrl = returnUrl, AppName = app.Name });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var ip = GetClientIp();

            // The hidden returnUrl field can be tampered with, so re-validate on every POST.
            var app = await _returnUrlValidator.ValidateAsync(model.ReturnUrl, ip);
            if (app == null)
            {
                return View("UnapprovedApp");
            }
            model.AppName = app.Name;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // ===== TASK 12: Log all login attempts to AuditLogs =====
                await _auditService.LogLoginAsync(null, model.Email, false, "Unknown email", ip);
                // ===== TASK 10: Handle failed login with clear error messages =====
                ModelState.AddModelError(string.Empty, InvalidCredentialsMessage);
                return View(model);
            }

            // ===== TASK 11: Rate limiting - lockoutOnFailure counts failures (5 attempts, 15 min lock; see Program.cs) =====


            // ===== TASK 6: Check IsActive flag =====
            if (!user.IsActive)
            {
                await _auditService.LogLoginAsync(user.Id, model.Email, false, "Account suspended", ip);
                ModelState.AddModelError(string.Empty, SuspendedMessage);
                return View(model);
            }

            // TASKS 7 & 8 live in JwtTokenService

            // ===== TASK 9: Redirect to returnUrl?token=jwt =====
        }

        private string? GetClientIp() => HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }
}