using MesEquipment.Api.DTOs;
using MesEquipment.Api.Data;
using MesEquipment.Api.Services;
using MesEquipment.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MesEquipment.Api.Authorization;

namespace MesEquipment.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MachinesController : ControllerBase
{
    private readonly IMachineService _machineService;

    public MachinesController(IMachineService machineService)
    {
        _machineService = machineService;
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.CanReadMachines)]
    public async Task<ActionResult<PagedResultDto<MachineDto>>> GetMachines(
        [FromQuery] MachineQueryDto query)
    {
        return await _machineService.GetPagedAsync(query);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = AuthorizationPolicies.CanReadMachines)]
    public async Task<ActionResult<MachineDto>> GetMachine(int id)
    {
        var machine = await _machineService.GetByIdAsync(id);

        if (machine == null)
        {
            return NotFound();
        }

        return machine;
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.CanCreateMachines)]
    public async Task<ActionResult<MachineDto>> CreateMachine(CreateMachineDto dto)
    {
        var machine = await _machineService.CreateAsync(dto);

        return CreatedAtAction(nameof(GetMachine), new { id = machine.Id }, machine);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = AuthorizationPolicies.CanUpdateMachines)]
    public async Task<IActionResult> UpdateMachine(int id, UpdateMachineDto dto)
    {
        var updated = await _machineService.UpdateAsync(id, dto);
        
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = AuthorizationPolicies.CanDeleteMachines)]
    public async Task<IActionResult> DeleteMachine(int id)
    {
        var deleted = await _machineService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
    
    [HttpGet("{id}/status-logs")]
    [Authorize(Policy = AuthorizationPolicies.CanReadMachines)]
    public async Task<ActionResult<IEnumerable<MachineStatusLogDto>>> GetMachineStatusLogs(int id)
    {
        var logs = await _machineService.GetStatusLogsAsync(id);

        return logs;
    }
}