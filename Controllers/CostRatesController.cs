using ClinicManagementSystem.Data;
using ClinicManagementSystem.Filters;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers;

[SessionAuthorize("Admin", "Operator")]
public class CostRatesController : Controller
{
    private readonly ClinicDbContext _db;
    public CostRatesController(ClinicDbContext db) => _db = db;

    public async Task<IActionResult> Index() =>
        View(await _db.CostRates.OrderBy(c => c.CostType).ThenBy(c => c.CostCode).ToListAsync());

    public IActionResult Create() => View(new CostRate());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CostRate model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _db.CostRates.AnyAsync(c => c.CostCode == model.CostCode))
        {
            ModelState.AddModelError(nameof(model.CostCode), "Cost code already exists.");
            return View(model);
        }

        _db.CostRates.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Cost rate created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.CostRates.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CostRate model)
    {
        if (id != model.CostRateId) return NotFound();
        if (!ModelState.IsValid) return View(model);

        _db.Update(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Cost rate updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [SessionAuthorize("Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.CostRates.FindAsync(id);
        if (item == null) return NotFound();

        item.IsActive = false;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Cost rate deactivated.";
        return RedirectToAction(nameof(Index));
    }
}

