using ITElectiveSSO.Models;
using ITELECTIVE_SSO.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tests
{
    public class UserManagementTests
    {
        private static UserManager<ApplicationUser> BuildUserManager(string dbName)
        {
            var services = new ServiceCollection();

            services.AddLogging();

            services.AddDbContext<SsoDbContext>(options =>
                options.UseInMemoryDatabase(dbName));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<SsoDbContext>()
            .AddDefaultTokenProviders();

            var provider = services.BuildServiceProvider();
            return provider.GetRequiredService<UserManager<ApplicationUser>>();
        }


        [Fact]
        public async Task CreateUser_WithValidData_CreatesUserSuccessfully()
        {
            // arranges
            var userManager = BuildUserManager(Guid.NewGuid().ToString());
            var newUser = new ApplicationUser
            {
                UserName = "newuser@example.com",
                Email = "newuser@example.com",
                IsActive = true
            };

            var result = await userManager.CreateAsync(newUser, "Password123");
            var createdUser = await userManager.FindByEmailAsync("newuser@example.com");

            // the asserts
            Assert.True(result.Succeeded);
            Assert.NotNull(createdUser);
            Assert.Equal("newuser@example.com", createdUser.Email);
            Assert.True(createdUser.IsActive);
        }


        [Fact]
        public async Task CreateUser_WithDuplicateEmail_FailsCreation()
        {
            // arranges
            var userManager = BuildUserManager(Guid.NewGuid().ToString());
            var firstUser = new ApplicationUser
            {
                UserName = "duplicate@example.com",
                Email = "duplicate@example.com",
                IsActive = true
            };
            var secondUser = new ApplicationUser
            {
                UserName = "duplicate2@example.com",
                Email = "duplicate@example.com",
                IsActive = true
            };

            // acts
            var firstResult = await userManager.CreateAsync(firstUser, "Password123");
            var secondResult = await userManager.CreateAsync(secondUser, "Password123");
            var totalUsersWithEmail = await userManager.Users
                .Where(u => u.Email == "duplicate@example.com")
                .CountAsync();

            // asserts
            Assert.True(firstResult.Succeeded);
            Assert.False(secondResult.Succeeded);
            Assert.Equal(1, totalUsersWithEmail);
        }

        [Fact]
        public async Task ToggleActive_ChangesIsActiveFromTrueToFalse()
        {
            // arrange
            var userManager = BuildUserManager(Guid.NewGuid().ToString());
            var user = new ApplicationUser
            {
                UserName = "toggleuser1@example.com",
                Email = "toggleuser1@example.com",
                IsActive = true
            };
            await userManager.CreateAsync(user, "Password123");

            // act
            user.IsActive = !user.IsActive;
            await userManager.UpdateAsync(user);
            var updatedUser = await userManager.FindByEmailAsync("toggleuser1@example.com");

            // assert
            Assert.NotNull(updatedUser);
            Assert.False(updatedUser.IsActive);
        }

        [Fact]
        public async Task ToggleActive_ChangesIsActiveFromFalseToTrue()
        {
            // arrange
            var userManager = BuildUserManager(Guid.NewGuid().ToString());
            var user = new ApplicationUser
            {
                UserName = "toggleuser2@example.com",
                Email = "toggleuser2@example.com",
                IsActive = false
            };
            await userManager.CreateAsync(user, "Password123");

            // act
            user.IsActive = !user.IsActive;
            await userManager.UpdateAsync(user);
            var updatedUser = await userManager.FindByEmailAsync("toggleuser2@example.com");

            // assert
            Assert.NotNull(updatedUser);
            Assert.True(updatedUser.IsActive);
        }
    }
}