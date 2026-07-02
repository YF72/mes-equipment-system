using System.ComponentModel.DataAnnotations;

namespace MesEquipment.Api.DTOs;

public class LoginDto
{
    [Required]
    [StringLength(100, MinimumLength = 5)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}