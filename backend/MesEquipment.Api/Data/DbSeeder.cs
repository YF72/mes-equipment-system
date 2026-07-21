using MesEquipment.Api.Authorization;
using MesEquipment.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MesEquipment.Api.Data;

public static class DbSeeder
{
    public static async Task SeedDevelopmentUsersAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        var demoUsers = new[]
        {
            ("admin", AppRoles.Administrator),
            ("ee", AppRoles.EquipmentEngineer),
            ("ee.manager", AppRoles.EquipmentManager),
            ("quality", AppRoles.QualityEngineer),
            ("eng", AppRoles.Engineering),
            ("pei", AppRoles.ProcessIntegrationEngineer)
        };

        foreach (var (username, role) in demoUsers)
        {
            var user = await dbContext.Users
                .SingleOrDefaultAsync(user => user.Username == username);

            if (user == null)
            {
                user = new User
                {
                    Username = username,
                    Role = role
                };

                user.PasswordHash = passwordHasher.HashPassword(user, "password");
                dbContext.Users.Add(user);
            }
            else if (user.Role != role)
            {
                user.Role = role;
            }
        }

        await dbContext.SaveChangesAsync();
    }
}