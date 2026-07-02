namespace MesEquipment.Api.Models;

public class MachineStatusLog
{
    public int Id { get; set; }

    public int MachineId { get; set; }

    public string OldStatus { get; set; } = string.Empty;

    public string NewStatus { get; set; } = string.Empty;

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    public string Note { get; set; } = string.Empty;

    public Machine? Machine { get; set; }
}