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

            // ===== TASK 3: Validate returnUrl against approved apps =====

            // ===== TASK 4: Show error if returnUrl invalid =====
            return View(new LoginViewModel { ReturnUrl = returnUrl, AppName = app.Name });
        }


        // TODO: whoever picks up Tasks 5, 6, 9, 10, 11, 12 adds the POST Login method here

        private string? GetClientIp() => HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }
}