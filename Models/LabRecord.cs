using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models;

public class LabRecord
{
    public int LabRecordId { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int? CheckInId { get; set; }
    public PatientCheckIn? CheckIn { get; set; }

    public int? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    [Required, StringLength(200)]
    public string TestName { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string TestResult { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    public DateTime TestDate { get; set; } = DateTime.Today;

    [StringLength(1000)]
    public string? Notes { get; set; }

    [StringLength(500)]
    public string? DocumentPath { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
