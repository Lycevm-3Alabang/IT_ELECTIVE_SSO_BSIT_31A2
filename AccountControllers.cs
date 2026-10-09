using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class AccountController : Controller
{
    private readonly IConfiguration _configuration;

    public AccountController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Login()
    {
        var baseUrl = _configuration["Sso:BaseUrl"];
        var callbackUrl = _configuration["Sso:CallbackUrl"];

        if (string.IsNullOrWhiteSpace(baseUrl) ||
            string.IsNullOrWhiteSpace(callbackUrl))
        {
            return Problem("SSO configuration is incomplete.");
        }

        var loginUrl = new Uri(
            new Uri(baseUrl.TrimEnd('/') + "/"),
            "Auth/Login");

        var builder = new UriBuilder(loginUrl);
        var query = System.Web.HttpUtility.ParseQueryString("");

        query["callbackUrl"] = callbackUrl;
        builder.Query = query.ToString();

        return Redirect(builder.ToString());
    }

    [HttpGet]
    public IActionResult Callback([FromQuery] string? token)
    {
        // TODO: Validate the token signature, expiry, issuer,
        // audience, and required claims using the actual Gateway
        // configuration before creating an authenticated cookie.
        //
        // Do not authenticate a user based on an unverified JWT.

        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized("Missing authentication token.");
        }

        return Problem(
            "JWT validation must be implemented before completing login.");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}