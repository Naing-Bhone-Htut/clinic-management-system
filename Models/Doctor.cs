using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models;

public class Doctor
{
    public int DoctorId { get; set; }

    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;

    [Required, StringLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Specialty { get; set; }

    [StringLength(100)]
    public string? Department { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<PatientCheckIn> CheckIns { get; set; } = new List<PatientCheckIn>();
    public ICollection<LabRecord> LabRecords { get; set; } = new List<LabRecord>();
}
