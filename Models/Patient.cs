using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models;

public class Patient
{
    public int PatientId { get; set; }

    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;

    [Required, StringLength(20)]
    public string PatientCode { get; set; } = string.Empty;

    [StringLength(10)]
    public string? BloodType { get; set; }

    [StringLength(500)]
    public string? Allergies { get; set; }

    [StringLength(1000)]
    public string? MedicalHistory { get; set; }

    public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<PatientDocument> Documents { get; set; } = new List<PatientDocument>();
    public ICollection<PatientCheckIn> CheckIns { get; set; } = new List<PatientCheckIn>();
    public ICollection<IncomeBill> IncomeBills { get; set; } = new List<IncomeBill>();
    public ICollection<LabRecord> LabRecords { get; set; } = new List<LabRecord>();
}
