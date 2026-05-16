using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.ViewModels;

public class CheckInFormViewModel
{
    public int? CheckInId { get; set; }

    [Required]
    public int PatientId { get; set; }

    public int? DoctorId { get; set; }

    [Required]
    public DateTime CheckInDateTime { get; set; } = DateTime.Now;

    public DateTime? CheckOutDateTime { get; set; }

    public bool IsServed { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

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

    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Patients { get; set; } = new();
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Doctors { get; set; } = new();
}
