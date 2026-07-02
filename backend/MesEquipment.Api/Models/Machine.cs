namespace MesEquipment.Api.Models;

public class Machine
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string Status { get; set; } = "Idle";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<MachineStatusLog> StatusLogs { get; set; } = [];
}