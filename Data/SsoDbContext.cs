using ITElectiveSSO.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ITELECTIVE_SSO.Data
{
    public class SsoDbContext : IdentityDbContext<ApplicationUser>
    {
        public SsoDbContext(DbContextOptions<SsoDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}