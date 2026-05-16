using System.ComponentModel.DataAnnotations;

namespace ClinicManagementSystem.Models;

public class ExpenseBillItem
{
    public int ExpenseBillItemId { get; set; }

    public int ExpenseBillId { get; set; }
    public ExpenseBill ExpenseBill { get; set; } = null!;

    public int CostRateId { get; set; }
    public CostRate CostRate { get; set; } = null!;

    [Range(1, 1000)]
    public int Quantity { get; set; } = 1;

    [Range(0, 99999999)]
    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }
}
