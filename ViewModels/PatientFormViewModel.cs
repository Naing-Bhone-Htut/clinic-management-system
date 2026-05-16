using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.ViewModels;

public class PatientFormViewModel
{
    public int? PatientId { get; set; }
    public int? PersonId { get; set; }

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

    [Phone, StringLength(20)]
    public string? Phone { get; set; }

    [EmailAddress, StringLength(100)]
    public string? Email { get; set; }

    [Required, StringLength(20)]
    public string PatientCode { get; set; } = string.Empty;

    [StringLength(10)]
    public string? BloodType { get; set; }

    [StringLength(500)]
    public string? Allergies { get; set; }

    [StringLength(1000)]
    public string? MedicalHistory { get; set; }

    [Required, StringLength(200)]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(200)]
    public string? AddressLine2 { get; set; }

    [Required, StringLength(100)]
    public string City { get; set; } = string.Empty;

    [StringLength(100)]
    public string? State { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    public int? AddressId { get; set; }
}
