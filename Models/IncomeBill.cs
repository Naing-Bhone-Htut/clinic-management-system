using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models;

public class IncomeBill
{
    public int IncomeBillId { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int? CheckInId { get; set; }
    public PatientCheckIn? CheckIn { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime BillDate { get; set; } = DateTime.Today;

    public decimal TotalAmount { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public int? CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }

    public ICollection<IncomeBillItem> Items { get; set; } = new List<IncomeBillItem>();
}
