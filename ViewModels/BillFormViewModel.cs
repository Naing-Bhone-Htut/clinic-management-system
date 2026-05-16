using System.ComponentModel.DataAnnotations;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.ViewModels;

public class BillFormViewModel
{
    public int? BillId { get; set; }

    [Required(ErrorMessage = "Please select a patient.")]
    public int? PatientId { get; set; }

    public int? CheckInId { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime BillDate { get; set; } = DateTime.Today;

    [StringLength(500)]
    public string? Notes { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public List<BillItemLineViewModel> Items { get; set; } = new() { new() };

    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Patients { get; set; } = new();
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> CostRates { get; set; } = new();
}

public class BillItemLineViewModel
{
    public int CostRateId { get; set; }

    [Range(1, 1000)]
    public int Quantity { get; set; } = 1;

    [Range(0, 99999999)]
    public decimal UnitPrice { get; set; }
}
