using MesEquipment.Api.DTOs;

namespace MesEquipment.Api.Services;

public interface IMachineService
{
    Task<PagedResultDto<MachineDto>> GetPagedAsync(MachineQueryDto query);

    Task<MachineDto?> GetByIdAsync(int id);

    Task<MachineDto> CreateAsync(CreateMachineDto dto);

    Task<bool> UpdateAsync(int id, UpdateMachineDto dto);

    Task<bool> DeleteAsync(int id);
    
    Task<List<MachineStatusLogDto>> GetStatusLogsAsync(int machineId);
}