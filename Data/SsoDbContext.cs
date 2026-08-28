using ITElectiveSSO.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ITELECTIVE_SSO.Data
{
    public class SsoDbContext : IdentityDbContext<ApplicationUser>
    {
        public SsoDbContext(DbContextOptions<SsoDbContext> options) : base(options) { }

        public DbSet<TenantApp> TenantApps { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<UserGroup> UserGroups { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TenantApp>(e =>
            {
                e.Property(t => t.Name).IsRequired();
                e.Property(t => t.ReturnUrl).IsRequired();
            });

            builder.Entity<Group>(e =>
            {
                e.HasOne(g => g.TenantApp)
                 .WithMany(t => t.Groups)
                 .HasForeignKey(g => g.TenantAppId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(g => new { g.TenantAppId, g.Name }).IsUnique();
            });

            builder.Entity<UserGroup>(e =>
            {
                e.HasKey(ug => new { ug.UserId, ug.GroupId });

                e.HasOne(ug => ug.User)
                 .WithMany(u => u.UserGroups)
                 .HasForeignKey(ug => ug.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(ug => ug.Group)
                 .WithMany(g => g.UserGroups)
                 .HasForeignKey(ug => ug.GroupId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<AuditLog>(e =>
            {
                e.HasOne(a => a.User)
                 .WithMany()
                 .HasForeignKey(a => a.UserId)
                 .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}