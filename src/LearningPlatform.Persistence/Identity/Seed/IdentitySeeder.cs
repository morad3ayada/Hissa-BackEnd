using System.Security.Cryptography;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LearningPlatform.Persistence.Identity.Seed;

/// <summary>
/// Ensures the platform roles and a default Admin account exist. Intended to be
/// invoked once at application startup (e.g. from Program.cs) after the database
/// is migrated.
/// </summary>
public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(IdentitySeeder));

        await EnsureRolesAsync(roleManager);
        await EnsureAdminUserAsync(userManager, configuration, logger);
    }

    private static async Task EnsureRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (var role in RoleSeedData.Roles)
        {
            if (await roleManager.RoleExistsAsync(role.Name!))
                continue;

            await roleManager.CreateAsync(new IdentityRole<Guid>(role.Name!)
            {
                Id = role.Id,
                NormalizedName = role.NormalizedName
            });
        }
    }

    private static async Task EnsureAdminUserAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        ILogger logger)
    {
        var adminEmail = configuration["Seed:AdminEmail"] ?? "admin@learningplatform.com";

        if (await userManager.FindByEmailAsync(adminEmail) is not null)
            return;

        var configuredPassword = configuration["Seed:AdminPassword"];
        var passwordWasGenerated = string.IsNullOrWhiteSpace(configuredPassword);
        var password = passwordWasGenerated ? GenerateSecurePassword() : configuredPassword!;

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Platform",
            LastName = "Admin",
            Role = UserRole.Admin,
            IsActive = true,
            EmailConfirmed = true
        };

        // UserManager.CreateAsync hashes the password via Identity's configured
        // IPasswordHasher<TUser> — never store or compare raw passwords manually.
        var result = await userManager.CreateAsync(admin, password);

        if (!result.Succeeded)
        {
            logger.LogError(
                "Failed to seed the default Admin account: {Errors}",
                string.Join("; ", result.Errors.Select(e => e.Description)));
            return;
        }

        await userManager.AddToRoleAsync(admin, UserRole.Admin.ToString());

        if (passwordWasGenerated)
        {
            logger.LogWarning(
                "A default Admin account was created for {Email} with a generated password: {Password}. " +
                "Sign in and change it immediately, or set Seed:AdminPassword before the next deployment.",
                adminEmail, password);
        }
    }

    private static string GenerateSecurePassword()
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnpqrstuvwxyz";
        const string digits = "23456789";
        const string special = "!@#$%^&*";
        const string all = upper + lower + digits + special;

        const int length = 16;
        var buffer = RandomNumberGenerator.GetBytes(length);
        var chars = new char[length];

        chars[0] = upper[buffer[0] % upper.Length];
        chars[1] = lower[buffer[1] % lower.Length];
        chars[2] = digits[buffer[2] % digits.Length];
        chars[3] = special[buffer[3] % special.Length];

        for (var i = 4; i < length; i++)
            chars[i] = all[buffer[i] % all.Length];

        for (var i = length - 1; i > 0; i--)
        {
            var j = buffer[i] % (i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }

        return new string(chars);
    }
}
