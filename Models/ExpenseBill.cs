using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models;

public class ExpenseBill
{
    public int ExpenseBillId { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime BillDate { get; set; } = DateTime.Today;

    public decimal TotalAmount { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public int? CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }

    public ICollection<ExpenseBillItem> Items { get; set; } = new List<ExpenseBillItem>();
}
