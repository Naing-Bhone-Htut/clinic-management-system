using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models;

public class Person
{
    public int PersonId { get; set; }

    [Required, StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? MiddleName { get; set; }

    [Required, StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, StringLength(10)]
    public string Gender { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [EmailAddress, StringLength(100)]
    public string? Email { get; set; }

    [StringLength(30)]
    public string? NationalId { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
    public Nurse? Nurse { get; set; }
    public OperatorStaff? Operator { get; set; }
    public ICollection<Address> Addresses { get; set; } = new List<Address>();

    public string FullName => $"{FirstName} {LastName}".Trim();
}
