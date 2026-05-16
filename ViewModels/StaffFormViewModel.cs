using System.ComponentModel.DataAnnotations;
using ClinicManagementSystem.Models.Enums;

namespace ClinicManagementSystem.ViewModels;

public class StaffFormViewModel
{
    public int? PersonId { get; set; }
    public int? StaffId { get; set; }
    public int? UserId { get; set; }

    [Required]
    public string StaffType { get; set; } = "Doctor";

    [Required, StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? MiddleName { get; set; }

    [Required, StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string Gender { get; set; } = "Male";

    [Required, DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-30);

    [Phone]
    public string? Phone { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(50)]
    public string? LicenseNumber { get; set; }

    [StringLength(100)]
    public string? Specialty { get; set; }

    [StringLength(100)]
    public string? Department { get; set; }

    [Required, StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public UserRole Role { get; set; } = UserRole.Doctor;

    public bool CreateUserAccount { get; set; } = true;
    public bool IsActive { get; set; } = true;
}
