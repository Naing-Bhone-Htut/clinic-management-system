using System.ComponentModel.DataAnnotations;
using ClinicManagementSystem.Models.Enums;

namespace ClinicManagementSystem.Models;

public class User
{
    public int UserId { get; set; }

    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;

    [Required, StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, StringLength(256)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
