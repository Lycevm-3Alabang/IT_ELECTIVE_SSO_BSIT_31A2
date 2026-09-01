using ITElectiveSSO.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ITELECTIVE_SSO.Data
{
    public static class SeedData
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var adminEmail = configuration["AdminSeed:Email"];
            var adminPassword = configuration["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                throw new InvalidOperationException(
                    "AdminSeed:Email and AdminSeed:Password must be set in appsettings.json.");
            }

            await SeedAdminAsync(userManager, adminEmail, adminPassword);
        }

        public static async Task SeedAdminAsync(
            UserManager<ApplicationUser> userManager,
            string adminEmail,
            string adminPassword)
        {
            // TODO (P3): idempotent check
            // TODO (P4): create admin via UserManager
            // TODO (P5): ensure IsActive = true
        }
    }
}