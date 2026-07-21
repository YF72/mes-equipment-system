using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MesEquipment.Api.Data;
using MesEquipment.Api.DTOs;
using MesEquipment.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MesEquipment.Api.Authorization;

namespace MesEquipment.Api.Tests.Integration;

public class MachinesApiTests
{
    [Fact]
    public async Task GetMachines_ReturnsUnauthorized_WhenTokenIsMissing()
    {
        await using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/Machines");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task GetMachines_ReturnsMachines_WhenTokenIsValid()
    {
        await using var factory = new CustomWebApplicationFactory();

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            var user = new User
            {
                Username = "admin",
                Role = AppRoles.Administrator,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = passwordHasher.HashPassword(user, "password");

            dbContext.Users.Add(user);

            dbContext.Machines.AddRange(
                new Machine
                {
                    Code = "M001",
                    Name = "CNC 1",
                    Location = "A1",
                    Status = "Idle"
                },
                new Machine
                {
                    Code = "M002",
                    Name = "Laser Cutter",
                    Location = "B1",
                    Status = "Running"
                }
            );

            await dbContext.SaveChangesAsync();
        }

        var client = factory.CreateClient();

        var loginResponse = await client.PostAsJsonAsync("/api/Auth/login", new LoginDto
        {
            Username = "admin",
            Password = "password"
        });

        loginResponse.EnsureSuccessStatusCode();

        var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        Assert.NotNull(loginBody);
        Assert.False(string.IsNullOrWhiteSpace(loginBody.Token));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginBody.Token);

        var response = await client.GetAsync("/api/Machines?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var machines = await response.Content.ReadFromJsonAsync<PagedResultDto<MachineDto>>();

        Assert.NotNull(machines);
        Assert.Equal(2, machines.TotalCount);
        Assert.Equal(2, machines.Items.Count);
    }
    
    [Fact]
    public async Task CreateMachine_ReturnsBadRequest_WhenStatusIsInvalid()
    {
        await using var factory = new CustomWebApplicationFactory();

        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            var user = new User
            {
                Username = "admin-validation",
                Role = AppRoles.Administrator,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = passwordHasher.HashPassword(user, "password");

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        var client = factory.CreateClient();

        var loginResponse = await client.PostAsJsonAsync("/api/Auth/login", new LoginDto
        {
            Username = "admin-validation",
            Password = "password"
        });

        loginResponse.EnsureSuccessStatusCode();

        var loginBody = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        Assert.NotNull(loginBody);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginBody.Token);

        var response = await client.PostAsJsonAsync("/api/Machines", new CreateMachineDto
        {
            Code = "M999",
            Name = "Invalid Status Machine",
            Location = "Z1",
            Status = "Broken"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}