using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models;

public class PatientCheckIn
{
    public int CheckInId { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    public DateTime CheckInDateTime { get; set; } = DateTime.Now;

    public DateTime? CheckOutDateTime { get; set; }

    public bool IsServed { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public int? CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }

    public VitalSign? VitalSign { get; set; }
    public ICollection<IncomeBill> IncomeBills { get; set; } = new List<IncomeBill>();
}
