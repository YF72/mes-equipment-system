using MesEquipment.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MesEquipment.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAdminUserAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        var adminExists = await dbContext.Users
            .AnyAsync(user => user.Username == "admin");

        if (adminExists)
        {
            return;
        }

        var admin = new User
        {
            Username = "admin"
        };

        admin.PasswordHash = passwordHasher.HashPassword(admin, "password");

        dbContext.Users.Add(admin);
        await dbContext.SaveChangesAsync();
    }
}