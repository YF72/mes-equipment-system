using MesEquipment.Api.Data;
using MesEquipment.Api.DTOs;
using MesEquipment.Api.Models;
using MesEquipment.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace MesEquipment.Api.Tests.Services;

public class MachineServiceTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsMachinesWithPagination()
    {
        using var context = CreateDbContext();

        context.Machines.AddRange(
            new Machine { Code = "M001", Name = "CNC 1", Location = "A1", Status = "Idle" },
            new Machine { Code = "M002", Name = "CNC 2", Location = "A2", Status = "Running" },
            new Machine { Code = "M003", Name = "Laser 1", Location = "B1", Status = "Down" }
        );

        await context.SaveChangesAsync();

        var service = new MachineService(context);

        var result = await service.GetPagedAsync(new MachineQueryDto
        {
            Page = 1,
            PageSize = 2
        });

        Assert.Equal(3, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("M001", result.Items[0].Code);
        Assert.Equal("M002", result.Items[1].Code);
    }

    [Fact]
    public async Task GetPagedAsync_FiltersByKeyword()
    {
        using var context = CreateDbContext();

        context.Machines.AddRange(
            new Machine { Code = "M001", Name = "CNC 1", Location = "A1", Status = "Idle" },
            new Machine { Code = "M002", Name = "Laser Cutter", Location = "B1", Status = "Running" }
        );

        await context.SaveChangesAsync();

        var service = new MachineService(context);

        var result = await service.GetPagedAsync(new MachineQueryDto
        {
            Keyword = "Laser",
            Page = 1,
            PageSize = 10
        });

        Assert.Single(result.Items);
        Assert.Equal("M002", result.Items[0].Code);
    }

    [Fact]
    public async Task GetPagedAsync_FiltersByStatus()
    {
        using var context = CreateDbContext();

        context.Machines.AddRange(
            new Machine { Code = "M001", Name = "CNC 1", Location = "A1", Status = "Idle" },
            new Machine { Code = "M002", Name = "CNC 2", Location = "A2", Status = "Running" }
        );

        await context.SaveChangesAsync();

        var service = new MachineService(context);

        var result = await service.GetPagedAsync(new MachineQueryDto
        {
            Status = "Running",
            Page = 1,
            PageSize = 10
        });

        Assert.Single(result.Items);
        Assert.Equal("M002", result.Items[0].Code);
        Assert.Equal("Running", result.Items[0].Status);
    }

    [Fact]
    public async Task CreateAsync_AddsMachineAndReturnsDto()
    {
        using var context = CreateDbContext();
        var service = new MachineService(context);

        var result = await service.CreateAsync(new CreateMachineDto
        {
            Code = "M010",
            Name = "Assembly Robot",
            Location = "C1",
            Status = "Idle"
        });

        var machineInDb = await context.Machines.FirstOrDefaultAsync(machine => machine.Code == "M010");

        Assert.NotNull(machineInDb);
        Assert.Equal("M010", result.Code);
        Assert.Equal("Assembly Robot", result.Name);
        Assert.Equal("Idle", result.Status);
    }
    
    [Fact]
    public async Task GetByIdAsync_ReturnsMachineWhenMachineExists()
    {
        using var context = CreateDbContext();

        var machine = new Machine
        {
            Code = "M020",
            Name = "Packaging Machine",
            Location = "D1",
            Status = "Running"
        };

        context.Machines.Add(machine);
        await context.SaveChangesAsync();

        var service = new MachineService(context);

        var result = await service.GetByIdAsync(machine.Id);

        Assert.NotNull(result);
        Assert.Equal(machine.Id, result.Id);
        Assert.Equal("M020", result.Code);
        Assert.Equal("Packaging Machine", result.Name);
    }
    
    [Fact]
    public async Task GetByIdAsync_ReturnsNullWhenMachineDoesNotExist()
    {
        using var context = CreateDbContext();
        var service = new MachineService(context);

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }
    
    [Fact]
    public async Task DeleteAsync_RemovesMachineWhenMachineExists()
    {
        using var context = CreateDbContext();

        var machine = new Machine
        {
            Code = "M030",
            Name = "Inspection Machine",
            Location = "E1",
            Status = "Idle"
        };

        context.Machines.Add(machine);
        await context.SaveChangesAsync();

        var service = new MachineService(context);

        var result = await service.DeleteAsync(machine.Id);

        var machineInDb = await context.Machines.FindAsync(machine.Id);

        Assert.True(result);
        Assert.Null(machineInDb);
    }
    
    [Fact]
    public async Task DeleteAsync_ReturnsFalseWhenMachineDoesNotExist()
    {
        using var context = CreateDbContext();
        var service = new MachineService(context);

        var result = await service.DeleteAsync(999);

        Assert.False(result);
    }
    
    [Fact]
    public async Task UpdateAsync_CreatesStatusLogWhenStatusChanges()
    {
        using var context = CreateDbContext();

        var machine = new Machine
        {
            Code = "M040",
            Name = "Press Machine",
            Location = "F1",
            Status = "Idle"
        };

        context.Machines.Add(machine);
        await context.SaveChangesAsync();

        var service = new MachineService(context);

        var result = await service.UpdateAsync(machine.Id, new UpdateMachineDto
        {
            Code = "M040",
            Name = "Press Machine",
            Location = "F1",
            Status = "Running"
        });

        var statusLog = await context.MachineStatusLogs.FirstOrDefaultAsync(log => log.MachineId == machine.Id);

        Assert.True(result);
        Assert.NotNull(statusLog);
        Assert.Equal("Idle", statusLog.OldStatus);
        Assert.Equal("Running", statusLog.NewStatus);
    }
    
    [Fact]
    public async Task UpdateAsync_DoesNotCreateStatusLogWhenStatusDoesNotChange()
    {
        using var context = CreateDbContext();

        var machine = new Machine
        {
            Code = "M050",
            Name = "Old Name",
            Location = "G1",
            Status = "Idle"
        };

        context.Machines.Add(machine);
        await context.SaveChangesAsync();

        var service = new MachineService(context);

        var result = await service.UpdateAsync(machine.Id, new UpdateMachineDto
        {
            Code = "M050",
            Name = "New Name",
            Location = "G2",
            Status = "Idle"
        });

        var statusLogs = await context.MachineStatusLogs
            .Where(log => log.MachineId == machine.Id)
            .ToListAsync();

        Assert.True(result);
        Assert.Empty(statusLogs);
    }
}