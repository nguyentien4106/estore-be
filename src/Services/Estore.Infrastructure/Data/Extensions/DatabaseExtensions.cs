using BuildingBlocks.Auth.Constants;
using BuildingBlocks.Auth.Models;
using EStore.Domain.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EStore.Infrastructure.Data.Extensions;

public static class DatabaseExtensions
{
    private static string[] Roles = ["Admin", "User"];

    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<EStoreDbContext>();

        context.Database.MigrateAsync().GetAwaiter().GetResult();

        await SeedAsync(context);

    }

    private static async Task SeedAsync(EStoreDbContext context)
    {
        await SeedRolesAsync(context);
        await SeedUsersAsync(context);
    }

    private static async Task SeedRolesAsync(EStoreDbContext context)
    {
        var roleManager = context.GetService<RoleManager<IdentityRole>>();

        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static async Task SeedUsersAsync(EStoreDbContext context)
    {
        if (!context.Users.Any(u => u.Email == "pro@gmail.com"))
        {
            var adminUser = new User
            {
                UserName = "pro@gmail.com",
                Email = "pro@gmail.com",
                EmailConfirmed = true,
                FirstName = "pro",
                LastName = "account",
                PhoneNumber = "1234567890",
                AccountType = AccountType.Pro,
                NormalizedEmail = "PRO@GMAIL.COM",
                NormalizedUserName = "PRO@GMAIL.COM"
            };

            var passwordHasher = new PasswordHasher<User>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Ti100600@");

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();

            var adminRole = context.Roles.FirstOrDefault(r => r.Name == Roles[0]);
            if (adminRole != null)
            {
                context.UserRoles.Add(new IdentityUserRole<string> { UserId = adminUser.Id, RoleId = adminRole.Id });
            }
        }

        if (!context.Users.Any(u => u.Email == "plus@gmail.com"))
        {
            var user = new User
            {
                UserName = "plus@gmail.com",
                Email = "plus@gmail.com",
                EmailConfirmed = true,
                FirstName = "plus",
                LastName = "account",
                PhoneNumber = "1234567890",
                AccountType = AccountType.Plus,
                NormalizedEmail = "PLUS@GMAIL.COM",
                NormalizedUserName = "PLUS@GMAIL.COM"
            };

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, "Ti100600@");

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var userRole = context.Roles.FirstOrDefault(r => r.Name == Roles[0]);
            if (userRole != null)
            {
                context.UserRoles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = userRole.Id });
            }
        }

        if (!context.Users.Any(u => u.Email == "free@gmail.com"))
        {
            var user = new User
            {
                UserName = "free@gmail.com",
                Email = "free@gmail.com",
                EmailConfirmed = true,
                FirstName = "free",
                LastName = "account",
                PhoneNumber = "1234567890",
                AccountType = AccountType.Free,
                NormalizedEmail = "FREE@GMAIL.COM",
                NormalizedUserName = "FREE@GMAIL.COM"
            };

            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, "Ti100600@");

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var userRole = context.Roles.FirstOrDefault(r => r.Name == Roles[1]);
            if (userRole != null)
            {
                context.UserRoles.Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = userRole.Id });
            }
        }
        await context.SaveChangesAsync();
    }
}
