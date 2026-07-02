using System.ComponentModel.DataAnnotations;

namespace MesEquipment.Api.DTOs;

public class MachineQueryDto
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;

    [StringLength(100)]
    public string? Keyword { get; set; }

    [RegularExpression("^(Idle|Running|Down|Maintenance)$",
        ErrorMessage = "Status must be one of: Idle, Running, Down, Maintenance.")]
    public string? Status { get; set; }
}