using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using System.Linq;
using Domain.Models;
using Microsoft.AspNetCore.DataProtection;

namespace Tests.TestUtilities
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test"); // Set the test environment

            builder.ConfigureServices(services =>
            {
                // ✅ Remove existing DataContext registrations
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DataContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var descriptor2 = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DataContext));

                if (descriptor2 != null)
                {
                    services.Remove(descriptor2);
                }

                // ✅ Use InMemory database for testing
                services.AddDbContext<DataContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });
                
                // Register controllers from API project
                services.AddControllers().AddApplicationPart(typeof(API.Controllers.BaseApiController).Assembly);
                
                // ✅ Register Identity Services
                services.AddIdentityCore<AppUser>(opt =>
                {
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.User.RequireUniqueEmail = true;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();

                // ✅ Register Data Protection (Fixes Identity Issue)
                services.AddDataProtection()
                    .SetApplicationName("DishCraftTests");

                // ✅ Mock authentication for testing
                services.AddAuthentication("TestAuth")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", options => { });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("TestPolicy", policy =>
                    {
                        policy.RequireAuthenticatedUser();
                    });
                });

                // ✅ Ensure in-memory DB is created and reset before each test
                var serviceProvider = services.BuildServiceProvider();
                using (var scope = serviceProvider.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<DataContext>();

                    // ✅ Reset the database before running tests
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();

                    // ✅ Add a test user so authentication works
                    var userManager = scopedServices.GetRequiredService<UserManager<AppUser>>();
                    var testUser = new AppUser
                    {
                        Id = "746d6986-cbf9-4221-8350-21f7daa42c7b",
                        UserName = "TestUser",
                        Email = "testuser@example.com"
                    };

                    if (userManager.FindByIdAsync(testUser.Id).Result == null)
                    {
                        userManager.CreateAsync(testUser, "TestPassword123!").Wait();
                    }
                }
            });
        }
    }
}
