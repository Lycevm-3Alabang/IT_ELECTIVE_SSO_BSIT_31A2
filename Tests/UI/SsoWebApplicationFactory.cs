using ITELECTIVE_SSO.Data;
using ITElectiveSSO.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Gateway.Tests
{
    /// <summary>
    /// Runs the real Gateway app for UI tests, but on a throw-away in-memory database
    /// that already contains one approved tenant app. This keeps the tests independent
    /// from the developer's sso_bsit31a2.db file.
    /// </summary>
    public class SsoWebApplicationFactory : WebApplicationFactory<Program>
    {
        public const string ApprovedReturnUrl = "https://sales.example.com/callback";

        // The login page only renders the form when returnUrl belongs to a registered app
        public static readonly string LoginUrl =
            "/Auth/Login?returnUrl=" + Uri.EscapeDataString(ApprovedReturnUrl);

        private readonly string _dbName = "ui-tests-" + Guid.NewGuid();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the SQLite registration made in Gateway/Program.cs.
                // (EF Core 9+ also registers an IDbContextOptionsConfiguration<T>, so match by name.)
                var toRemove = services
                    .Where(d =>
                        d.ServiceType == typeof(DbContextOptions<SsoDbContext>) ||
                        d.ServiceType == typeof(DbContextOptions) ||
                        (d.ServiceType.IsGenericType &&
                         d.ServiceType.Name.StartsWith("IDbContextOptionsConfiguration") &&
                         d.ServiceType.GetGenericArguments().Contains(typeof(SsoDbContext))))
                    .ToList();

                foreach (var descriptor in toRemove)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<SsoDbContext>(options => options.UseInMemoryDatabase(_dbName));
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SsoDbContext>();
            db.Database.EnsureCreated();

            if (!db.TenantApps.Any(a => a.ReturnUrl == ApprovedReturnUrl))
            {
                db.TenantApps.Add(new TenantApp
                {
                    Name = "SalesApp",
                    ReturnUrl = ApprovedReturnUrl,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
                db.SaveChanges();
            }

            return host;
        }
    }
}