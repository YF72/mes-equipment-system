using MesEquipment.Api.Data;
using MesEquipment.Api.DTOs;
using MesEquipment.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MesEquipment.Api.Services;

public class MachineService : IMachineService
{
    private readonly AppDbContext _context;

    public MachineService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<MachineDto>> GetPagedAsync(MachineQueryDto query)
    {
        var machinesQuery = _context.Machines.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();

            machinesQuery = machinesQuery.Where(machine =>
                machine.Code.Contains(keyword) ||
                machine.Name.Contains(keyword) ||
                machine.Location.Contains(keyword));
        }
        
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            machinesQuery = machinesQuery.Where(machine => machine.Status == query.Status);
        }
        var totalCount = await machinesQuery.CountAsync();
        var items = await machinesQuery
            .OrderBy(machine => machine.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(machine => new MachineDto
            {
                Id = machine.Id,
                Code = machine.Code,
                Name = machine.Name,
                Location = machine.Location,
                Status = machine.Status,
                CreatedAt = machine.CreatedAt
            })
            .ToListAsync();
        
        return new PagedResultDto<MachineDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<MachineDto?> GetByIdAsync(int id)
    {
        return await _context.Machines
            .Where(machine => machine.Id == id)
            .Select(machine => new MachineDto
            {
                Id = machine.Id,
                Code = machine.Code,
                Name = machine.Name,
                Location = machine.Location,
                Status = machine.Status,
                CreatedAt = machine.CreatedAt
            })
            .FirstOrDefaultAsync();
    }

    public async Task<MachineDto> CreateAsync(CreateMachineDto dto)
    {
        var machine = new Machine
        {
            Code = dto.Code,
            Name = dto.Name,
            Location = dto.Location,
            Status = dto.Status
        };

        _context.Machines.Add(machine);
        await _context.SaveChangesAsync();

        return new MachineDto
        {
            Id = machine.Id,
            Code = machine.Code,
            Name = machine.Name,
            Location = machine.Location,
            Status = machine.Status,
            CreatedAt = machine.CreatedAt
        };
    }

    public async Task<bool> UpdateAsync(int id, UpdateMachineDto dto)
    {
        var machine = await _context.Machines.FindAsync(id);

        if (machine == null)
        {
            return false;
        }
        
        var oldStatus = machine.Status;

        machine.Code = dto.Code;
        machine.Name = dto.Name;
        machine.Location = dto.Location;
        machine.Status = dto.Status;

        if (oldStatus != dto.Status)
        {
            var statusLog = new MachineStatusLog
            {
                MachineId = machine.Id,
                OldStatus = oldStatus,
                NewStatus = dto.Status,
                ChangedAt = DateTime.UtcNow,
                Note = "Status changed from " + oldStatus + " to " + dto.Status,
            };
            
            _context.MachineStatusLogs.Add(statusLog);
        }
        
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var machine = await _context.Machines.FindAsync(id);

        if (machine == null)
        {
            return false;
        }

        _context.Machines.Remove(machine);
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<List<MachineStatusLogDto>> GetStatusLogsAsync(int machineId)
    {
        return await _context.MachineStatusLogs
            .Where(log => log.MachineId == machineId)
            .OrderByDescending(log => log.ChangedAt)
            .Select(log => new MachineStatusLogDto
            {
                Id = log.Id,
                MachineId = log.MachineId,
                OldStatus = log.OldStatus,
                NewStatus = log.NewStatus,
                ChangedAt = log.ChangedAt,
                Note = log.Note
            })
            .ToListAsync();
    }
}