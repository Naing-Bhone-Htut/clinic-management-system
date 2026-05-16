using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models;

public class Nurse
{
    public int NurseId { get; set; }

    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;

    [StringLength(50)]
    public string? LicenseNumber { get; set; }

    [StringLength(100)]
    public string? Department { get; set; }

    public bool IsActive { get; set; } = true;
}
