using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MesEquipment.Api.Authorization;
using MesEquipment.Api.Data;
using MesEquipment.Api.DTOs;
using MesEquipment.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace MesEquipment.Api.Tests.Integration;

public class MachineAuthorizationTests
{
    [Fact]
    public async Task GetMachines_ReturnsOk_WhenUserHasQualityRole()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = await CreateAuthenticatedClientAsync(
            factory,
            "quality-test",
            AppRoles.QualityEngineer);

        var response = await client.GetAsync("/api/Machines");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateMachine_ReturnsForbidden_WhenUserHasQualityRole()
    {
        await using var factory = new CustomWebApplicationFactory();
        using var client = await CreateAuthenticatedClientAsync(
            factory,
            "quality-create-test",
            AppRoles.QualityEngineer);

        var response = await client.PostAsJsonAsync(
            "/api/Machines",
            new CreateMachineDto
            {
                Code = "Q001",
                Name = "Quality Test Machine",
                Location = "Q1",
                Status = "Idle"
            });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateMachine_ReturnsNoContent_WhenUserHasEngineeringRole()
    {
        await using var factory = new CustomWebApplicationFactory();
        var machineId = await SeedMachineAsync(factory);

        using var client = await CreateAuthenticatedClientAsync(
            factory,
            "engineering-update-test",
            AppRoles.Engineering);

        var response = await client.PutAsJsonAsync(
            $"/api/Machines/{machineId}",
            new UpdateMachineDto
            {
                Code = "ENG001",
                Name = "Engineering Test Machine",
                Location = "E1",
                Status = "Running"
            });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMachine_ReturnsForbidden_WhenUserHasEngineeringRole()
    {
        await using var factory = new CustomWebApplicationFactory();
        var machineId = await SeedMachineAsync(factory);

        using var client = await CreateAuthenticatedClientAsync(
            factory,
            "engineering-delete-test",
            AppRoles.Engineering);

        var response = await client.DeleteAsync($"/api/Machines/{machineId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private static async Task<HttpClient> CreateAuthenticatedClientAsync(
        CustomWebApplicationFactory factory,
        string username,
        string role)
    {
        using (var scope = factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher =
                scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            var user = new User
            {
                Username = username,
                Role = role,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = passwordHasher.HashPassword(user, "password");

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }

        var client = factory.CreateClient();

        var loginResponse = await client.PostAsJsonAsync(
            "/api/Auth/login",
            new LoginDto
            {
                Username = username,
                Password = "password"
            });

        loginResponse.EnsureSuccessStatusCode();

        var loginBody =
            await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        Assert.NotNull(loginBody);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginBody.Token);

        return client;
    }

    private static async Task<int> SeedMachineAsync(
        CustomWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var machine = new Machine
        {
            Code = $"TEST-{Guid.NewGuid():N}",
            Name = "Authorization Test Machine",
            Location = "Test Area",
            Status = "Idle"
        };

        dbContext.Machines.Add(machine);
        await dbContext.SaveChangesAsync();

        return machine.Id;
    }
}