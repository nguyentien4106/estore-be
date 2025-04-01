using EStore.Domain.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EStore.Infrastructure.Data.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<EStoreDbContext>();

        //context.Database.MigrateAsync().GetAwaiter().GetResult();

        //await SeedAsync(context);

    }

    private static async Task SeedAsync(EStoreDbContext context)
    {
        await SeedRolesAsync(context);
        await SeedUsersAsync(context);
    }

    private static async Task SeedRolesAsync(EStoreDbContext context)
    {
        var roleManager = context.GetService<RoleManager<IdentityRole>>();

        string[] roles = { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static async Task SeedUsersAsync(EStoreDbContext context)
    {
        if (!context.Users.Any(u => u.Email == "admin@example.com"))
        {
            var adminUser = new User
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "Admin",
                PhoneNumber = "1234567890",
            };

            var passwordHasher = new PasswordHasher<User>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Ti100600@");

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();

            var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
            if (adminRole != null)
            {
                context.UserRoles.Add(new IdentityUserRole<string> { UserId = adminUser.Id, RoleId = adminRole.Id });
            }
        }

        if (!context.Users.Any(u => u.Email == "user@example.com"))
        {
            var user = new User
            {
                UserName = "user@example.com",
                Email = "user@example.com",
                EmailConfirmed = true,
                FirstName = "User",
                LastName = "User",
                PhoneNumber = "1234567890",
            };

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, "Ti100600@");

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var userRole = context.Roles.FirstOrDefault(r => r.Name == "User");
            if (userRole != null)
            {
                context.UserRoles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = userRole.Id });
            }
        }

        await context.SaveChangesAsync();
    }
}
