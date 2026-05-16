using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models;

public class OperatorStaff
{
    public int OperatorId { get; set; }

    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;

    [StringLength(100)]
    public string? Department { get; set; }

    public bool IsActive { get; set; } = true;
}
