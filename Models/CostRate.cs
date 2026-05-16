using System.ComponentModel.DataAnnotations;
using ClinicManagementSystem.Models.Enums;

namespace ClinicManagementSystem.Models;

public class CostRate
{
    public int CostRateId { get; set; }

    [Required, StringLength(20)]
    public string CostCode { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [Required, Range(0, 99999999)]
    public decimal CostAmount { get; set; }

    [Required]
    public CostType CostType { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<IncomeBillItem> IncomeBillItems { get; set; } = new List<IncomeBillItem>();
    public ICollection<ExpenseBillItem> ExpenseBillItems { get; set; } = new List<ExpenseBillItem>();
}
