using System.Net;
using System.Net.Http.Json;
using MesEquipment.Api.Data;
using MesEquipment.Api.DTOs;
using MesEquipment.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace MesEquipment.Api.Tests.Integration;

public class AuthApiTests
{
    [Fact]
    public async Task Login_ReturnsToken_WhenCredentialsAreValid()
    {
        await using var factory = new CustomWebApplicationFactory();

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            var user = new User
            {
                Username = "admin",
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = passwordHasher.HashPassword(user, "password");

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/Auth/login", new LoginDto
        {
            Username = "admin",
            Password = "password"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

        Assert.NotNull(loginResponse);
        Assert.False(string.IsNullOrWhiteSpace(loginResponse.Token));
        Assert.Equal("admin", loginResponse.Username);
    }
}