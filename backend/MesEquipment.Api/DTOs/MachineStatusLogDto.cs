namespace MesEquipment.Api.DTOs;

public class MachineStatusLogDto
{
    public int Id { get; set; }

    public int MachineId { get; set; }

    public string OldStatus { get; set; } = string.Empty;

    public string NewStatus { get; set; } = string.Empty;

    public DateTime ChangedAt { get; set; }

    public string Note { get; set; } = string.Empty;
}