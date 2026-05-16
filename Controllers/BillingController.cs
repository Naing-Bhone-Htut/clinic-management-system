using ClinicManagementSystem.Data;
using ClinicManagementSystem.Filters;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.Enums;
using ClinicManagementSystem.Services;
using ClinicManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers;

[SessionAuthorize("Admin", "Operator")]
public class BillingController : Controller
{
    private readonly ClinicDbContext _db;
    public BillingController(ClinicDbContext db) => _db = db;

    public async Task<IActionResult> IncomeIndex()
    {
        var bills = await _db.IncomeBills
            .Include(b => b.Patient).ThenInclude(p => p.Person)
            .OrderByDescending(b => b.BillDate)
            .ToListAsync();
        return View(bills);
    }

    public async Task<IActionResult> ExpenseIndex()
    {
        var bills = await _db.ExpenseBills.OrderByDescending(b => b.BillDate).ToListAsync();
        return View(bills);
    }

    public async Task<IActionResult> CreateIncome() =>
        View(await BuildIncomeFormAsync(new BillFormViewModel()));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateIncome(BillFormViewModel model)
    {
        if (!ModelState.IsValid || model.PatientId is null or <= 0)
            return View(await BuildIncomeFormAsync(model));
        var validItems = model.Items?.Where(i => i.CostRateId > 0).ToList() ?? new();
        if (validItems.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Add at least one bill item with a cost code selected.");
            return View(await BuildIncomeFormAsync(model));
        }
        var bill = new IncomeBill
        {
            PatientId = model.PatientId.Value,
            CheckInId = model.CheckInId,
            BillDate = model.BillDate,
            Notes = model.Notes,
            CreatedByUserId = HttpContext.Session.GetInt32(SessionKeys.UserId)
        };
        _db.IncomeBills.Add(bill);
        await _db.SaveChangesAsync();
        decimal total = 0;
        foreach (var line in validItems)
        {
            var rate = await _db.CostRates.FindAsync(line.CostRateId);
            if (rate == null) continue;
            var lineTotal = line.Quantity * (line.UnitPrice > 0 ? line.UnitPrice : rate.CostAmount);
            total += lineTotal;
            _db.IncomeBillItems.Add(new IncomeBillItem
            {
                IncomeBillId = bill.IncomeBillId,
                CostRateId = line.CostRateId,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice > 0 ? line.UnitPrice : rate.CostAmount,
                LineTotal = lineTotal
            });
        }
        bill.TotalAmount = total;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Income bill created.";
        return RedirectToAction(nameof(IncomeIndex));
    }

    public async Task<IActionResult> IncomeDetails(int id)
    {
        var bill = await _db.IncomeBills
            .Include(b => b.Patient).ThenInclude(p => p.Person)
            .Include(b => b.Items).ThenInclude(i => i.CostRate)
            .FirstOrDefaultAsync(b => b.IncomeBillId == id);
        if (bill == null) return NotFound();
        return View(bill);
    }

    public async Task<IActionResult> CreateExpense() =>
        View(await BuildExpenseFormAsync(new BillFormViewModel()));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateExpense(BillFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(await BuildExpenseFormAsync(model));
        var validExpenseItems = model.Items?.Where(i => i.CostRateId > 0).ToList() ?? new();
        if (validExpenseItems.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Add at least one bill item with a cost code selected.");
            return View(await BuildExpenseFormAsync(model));
        }
        var bill = new ExpenseBill
        {
            BillDate = model.BillDate,
            Description = model.Description,
            CreatedByUserId = HttpContext.Session.GetInt32(SessionKeys.UserId)
        };
        _db.ExpenseBills.Add(bill);
        await _db.SaveChangesAsync();
        decimal total = 0;
        foreach (var line in validExpenseItems)
        {
            var rate = await _db.CostRates.FindAsync(line.CostRateId);
            if (rate == null) continue;
            var lineTotal = line.Quantity * (line.UnitPrice > 0 ? line.UnitPrice : rate.CostAmount);
            total += lineTotal;
            _db.ExpenseBillItems.Add(new ExpenseBillItem
            {
                ExpenseBillId = bill.ExpenseBillId,
                CostRateId = line.CostRateId,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice > 0 ? line.UnitPrice : rate.CostAmount,
                LineTotal = lineTotal
            });
        }
        bill.TotalAmount = total;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Expense bill created.";
        return RedirectToAction(nameof(ExpenseIndex));
    }

    public async Task<IActionResult> ExpenseDetails(int id)
    {
        var bill = await _db.ExpenseBills
            .Include(b => b.Items).ThenInclude(i => i.CostRate)
            .FirstOrDefaultAsync(b => b.ExpenseBillId == id);
        if (bill == null) return NotFound();
        return View(bill);
    }

    private async Task<BillFormViewModel> BuildIncomeFormAsync(BillFormViewModel model)
    {
        model.Patients = await _db.Patients.Include(p => p.Person)
            .Select(p => new SelectListItem($"{p.PatientCode} - {p.Person.FirstName} {p.Person.LastName}", p.PatientId.ToString()))
            .ToListAsync();
        model.CostRates = await _db.CostRates.Where(c => c.CostType == CostType.Income && c.IsActive)
            .Select(c => new SelectListItem($"{c.CostCode} - {c.Description} ({c.CostAmount:N0})", c.CostRateId.ToString()))
            .ToListAsync();
        return model;
    }

    private async Task<BillFormViewModel> BuildExpenseFormAsync(BillFormViewModel model)
    {
        model.CostRates = await _db.CostRates.Where(c => c.CostType == CostType.Expense && c.IsActive)
            .Select(c => new SelectListItem($"{c.CostCode} - {c.Description} ({c.CostAmount:N0})", c.CostRateId.ToString()))
            .ToListAsync();
        return model;
    }
}

