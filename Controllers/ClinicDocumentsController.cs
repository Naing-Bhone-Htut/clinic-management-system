using ClinicManagementSystem.Data;
using ClinicManagementSystem.Filters;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers;

[SessionAuthorize("Admin", "Operator", "Doctor", "Nurse")]
public class ClinicDocumentsController : Controller
{
    private readonly ClinicDbContext _db;
    private readonly FileUploadService _files;

    public ClinicDocumentsController(ClinicDbContext db, FileUploadService files)
    {
        _db = db;
        _files = files;
    }

    public async Task<IActionResult> Index() =>
        View(await _db.ClinicDocuments.OrderByDescending(d => d.UploadedDate).ToListAsync());

    [SessionAuthorize("Admin", "Operator")]
    public IActionResult Upload() => View();

    [HttpPost, ValidateAntiForgeryToken]
    [SessionAuthorize("Admin", "Operator")]
    public async Task<IActionResult> Upload(string title, string? description, IFormFile file)
    {
        if (string.IsNullOrWhiteSpace(title) || file == null || file.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Title and file are required.");
            return View();
        }

        var path = await _files.SaveFileAsync(file, "clinic");
        _db.ClinicDocuments.Add(new ClinicDocument
        {
            Title = title,
            Description = description,
            FilePath = path,
            FileName = file.FileName,
            UploadedByUserId = HttpContext.Session.GetInt32(SessionKeys.UserId)
        });
        await _db.SaveChangesAsync();
        TempData["Success"] = "Document uploaded.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Download(int id)
    {
        var doc = await _db.ClinicDocuments.FindAsync(id);
        if (doc == null) return NotFound();

        var physical = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", doc.FilePath.TrimStart('/'));
        if (!System.IO.File.Exists(physical)) return NotFound();
        return PhysicalFile(physical, "application/octet-stream", doc.FileName);
    }

    [SessionAuthorize("Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var doc = await _db.ClinicDocuments.FindAsync(id);
        if (doc == null) return NotFound();
        _db.ClinicDocuments.Remove(doc);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Document deleted.";
        return RedirectToAction(nameof(Index));
    }
}

