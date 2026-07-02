using System.ComponentModel.DataAnnotations;
namespace MesEquipment.Api.DTOs;

public class UpdateMachineDto
{
    [Required]
    [StringLength((50))]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength((100))]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength((100))]
    public string Location { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Idle|Running|Down|Maintenance)$", ErrorMessage = "Status must be one of: Idle, Running, Down, Maintenance.")]
    public string Status { get; set; } = "Idle";
}