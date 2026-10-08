using Gateway.Models;
using Gateway.Services;
using ITElectiveSSO.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Gateway.Controllers
{
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl)
        {
            // 1. If empty or attempting to redirect to Auth actions, default to Admin dashboard
            if (string.IsNullOrWhiteSpace(returnUrl) || returnUrl.StartsWith("/Auth", StringComparison.OrdinalIgnoreCase))
            {
                return View(new LoginViewModel { ReturnUrl = "/Admin/Index", AppName = "SSO Portal" });
            }

            // 2. Allow valid internal local URLs (e.g., /Admin/Index)
            if (Url.IsLocalUrl(returnUrl))
            {
                return View(new LoginViewModel { ReturnUrl = returnUrl, AppName = "SSO Portal" });
            }

            // 3. Validate external tenant applications
            var app = await _returnUrlValidator.ValidateAsync(returnUrl, GetClientIp());
            if (app == null)
            {
                return View("UnapprovedApp");
            }

            return View(new LoginViewModel { ReturnUrl = returnUrl, AppName = app.Name });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var ip = GetClientIp();
            TenantApp? app = null;

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && !model.ReturnUrl.StartsWith("/"))
            {
                app = await _returnUrlValidator.ValidateAsync(model.ReturnUrl, ip);
                if (app == null)
                {
                    return View("UnapprovedApp");
                }
                model.AppName = app.Name;
            }
            else
            {
                model.AppName = "SSO Portal";
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                await _auditService.LogLoginAsync(null, model.Email, false, "Unknown email", ip);
                ModelState.AddModelError(string.Empty, InvalidCredentialsMessage);
                return View(model);
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
            {
                await _auditService.LogLoginAsync(user.Id, model.Email, false, "Account locked out", ip);
                ModelState.AddModelError(string.Empty, LockedOutMessage);
                return View(model);
            }

            if (!signInResult.Succeeded)
            {
                await _auditService.LogLoginAsync(user.Id, model.Email, false, "Invalid password", ip);
                ModelState.AddModelError(string.Empty, InvalidCredentialsMessage);
                return View(model);
            }

            if (!user.IsActive)
            {
                await _auditService.LogLoginAsync(user.Id, model.Email, false, "Account suspended", ip);
                ModelState.AddModelError(string.Empty, SuspendedMessage);
                return View(model);
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // Writes authentication cookie to the browser so User claims persist
            await _signInManager.SignInAsync(user, isPersistent: false);

            await _auditService.LogLoginAsync(user.Id, user.Email!, true, null, ip);

            // External tenant application redirect
            if (app != null)
            {
                var token = await _jwtTokenService.CreateTokenAsync(user, app);
                return Redirect(QueryHelpers.AddQueryString(app.ReturnUrl, "token", token));
            }

            // Internal root controller redirect
            if (string.IsNullOrWhiteSpace(model.ReturnUrl) || model.ReturnUrl.StartsWith("/Admin") || model.ReturnUrl.StartsWith("/Auth"))
            {
                return RedirectToAction("Index", "Admin");
            }

            return LocalRedirect(model.ReturnUrl);
        }

        [HttpGet]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // 1. Fetch user directly via UserManager
            var user = await _userManager.GetUserAsync(User);

            // 2. Extract UserId with fallbacks
            var userId = user?.Id
                         ?? _userManager.GetUserId(User)
                         ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // 3. Extract Email with comprehensive claim fallbacks
            var email = user?.Email
                        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                        ?? User.Identity?.Name
                        ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
                        ?? User.FindFirst("email")?.Value
                        ?? "Unknown";

            var ipAddress = GetClientIp() ?? "127.0.0.1";

            // Record audit log entry
            await _auditService.LogLogoutAsync(userId, email, ipAddress);

            // Sign out user session
            await _signInManager.SignOutAsync();

            // Redirect back to Login with no returnUrl attached
            return RedirectToAction(nameof(Login));
        }

        private string? GetClientIp() => HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }
}
