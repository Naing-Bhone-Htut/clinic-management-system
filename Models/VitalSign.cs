using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models;

public class VitalSign
{
    public int VitalSignId { get; set; }

    public int CheckInId { get; set; }
    public PatientCheckIn CheckIn { get; set; } = null!;

    [StringLength(20)]
    public string? BloodPressure { get; set; }

    [Range(30, 45)]
    public decimal? Temperature { get; set; }

    [Range(30, 250)]
    public int? PulseRate { get; set; }

    [Range(5, 60)]
    public int? RespiratoryRate { get; set; }

    [Range(1, 500)]
    public decimal? Weight { get; set; }

    [Range(30, 250)]
    public decimal? Height { get; set; }

    public DateTime RecordedAt { get; set; } = DateTime.Now;

    public int? RecordedByUserId { get; set; }
    public User? RecordedByUser { get; set; }
}
