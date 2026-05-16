using ClinicManagementSystem.Data;
using ClinicManagementSystem.Filters;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers;

[SessionAuthorize("Admin", "Doctor", "Nurse", "Operator")]
public class LabRecordsController : Controller
{
    private readonly ClinicDbContext _db;
    private readonly FileUploadService _files;

    public LabRecordsController(ClinicDbContext db, FileUploadService files)
    {
        _db = db;
        _files = files;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var query = _db.LabRecords
            .Include(l => l.Patient).ThenInclude(p => p.Person)
            .Include(l => l.Doctor).ThenInclude(d => d!.Person)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(l => l.TestName.Contains(search) || l.Patient.Person.FirstName.Contains(search));

        ViewBag.Search = search;
        return View(await query.OrderByDescending(l => l.TestDate).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Patients = await GetPatientsAsync();
        ViewBag.Doctors = await GetDoctorsAsync();
        return View(new LabRecord { TestDate = DateTime.Today });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LabRecord model, IFormFile? document)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Patients = await GetPatientsAsync();
            ViewBag.Doctors = await GetDoctorsAsync();
            return View(model);
        }

        if (document != null && document.Length > 0)
            model.DocumentPath = await _files.SaveFileAsync(document, "lab");

        _db.LabRecords.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Lab record saved.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var record = await _db.LabRecords.FindAsync(id);
        if (record == null) return NotFound();
        ViewBag.Patients = await GetPatientsAsync();
        ViewBag.Doctors = await GetDoctorsAsync();
        return View(record);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LabRecord model, IFormFile? document)
    {
        if (id != model.LabRecordId) return NotFound();
        if (!ModelState.IsValid)
        {
            ViewBag.Patients = await GetPatientsAsync();
            ViewBag.Doctors = await GetDoctorsAsync();
            return View(model);
        }

        if (document != null && document.Length > 0)
            model.DocumentPath = await _files.SaveFileAsync(document, "lab");

        _db.Update(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Lab record updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Download(int id)
    {
        var record = await _db.LabRecords.FindAsync(id);
        if (record?.DocumentPath == null) return NotFound();

        var physical = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", record.DocumentPath.TrimStart('/'));
        if (!System.IO.File.Exists(physical)) return NotFound();
        return PhysicalFile(physical, "application/octet-stream", Path.GetFileName(physical));
    }

    [SessionAuthorize("Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var record = await _db.LabRecords.FindAsync(id);
        if (record == null) return NotFound();
        _db.LabRecords.Remove(record);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Lab record deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>> GetPatientsAsync() =>
        await _db.Patients.Include(p => p.Person)
            .Select(p => new SelectListItem($"{p.PatientCode} - {p.Person.FirstName} {p.Person.LastName}", p.PatientId.ToString()))
            .ToListAsync();

    private async Task<List<SelectListItem>> GetDoctorsAsync() =>
        await _db.Doctors.Include(d => d.Person)
            .Select(d => new SelectListItem(d.Person.FullName, d.DoctorId.ToString()))
            .ToListAsync();
}

