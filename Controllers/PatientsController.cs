using ClinicManagementSystem.Data;
using ClinicManagementSystem.Filters;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Services;
using ClinicManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers;

[SessionAuthorize("Admin", "Operator", "Nurse", "Doctor")]
public class PatientsController : Controller
{
    private readonly ClinicDbContext _db;
    private readonly FileUploadService _files;

    public PatientsController(ClinicDbContext db, FileUploadService files)
    {
        _db = db;
        _files = files;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var query = _db.Patients.Include(p => p.Person).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(p =>
                p.PatientCode.Contains(search) ||
                p.Person.FirstName.Contains(search) ||
                p.Person.LastName.Contains(search) ||
                (p.Person.Phone != null && p.Person.Phone.Contains(search)));
        }

        ViewBag.Search = search;
        var patients = await query.OrderByDescending(p => p.RegistrationDate).ToListAsync();
        return View(patients);
    }

    public async Task<IActionResult> Details(int id)
    {
        var patient = await _db.Patients
            .Include(p => p.Person)
            .Include(p => p.Addresses)
            .Include(p => p.Documents)
            .Include(p => p.CheckIns).ThenInclude(c => c.Doctor).ThenInclude(d => d!.Person)
            .FirstOrDefaultAsync(p => p.PatientId == id);

        if (patient == null) return NotFound();
        return View(patient);
    }

    [SessionAuthorize("Admin", "Operator")]
    public IActionResult Create() => View(new PatientFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    [SessionAuthorize("Admin", "Operator")]
    public async Task<IActionResult> Create(PatientFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _db.Patients.AnyAsync(p => p.PatientCode == model.PatientCode))
        {
            ModelState.AddModelError(nameof(model.PatientCode), "Patient code already exists.");
            return View(model);
        }

        var person = new Person
        {
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            LastName = model.LastName,
            Gender = model.Gender,
            DateOfBirth = model.DateOfBirth,
            Phone = model.Phone,
            Email = model.Email
        };
        _db.Persons.Add(person);
        await _db.SaveChangesAsync();

        var patient = new Patient
        {
            PersonId = person.PersonId,
            PatientCode = model.PatientCode,
            BloodType = model.BloodType,
            Allergies = model.Allergies,
            MedicalHistory = model.MedicalHistory
        };
        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();

        _db.Addresses.Add(new Address
        {
            PatientId = patient.PatientId,
            AddressLine1 = model.AddressLine1,
            AddressLine2 = model.AddressLine2,
            City = model.City,
            State = model.State,
            PostalCode = model.PostalCode
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Patient created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [SessionAuthorize("Admin", "Operator")]
    public async Task<IActionResult> Edit(int id)
    {
        var patient = await _db.Patients.Include(p => p.Person).Include(p => p.Addresses)
            .FirstOrDefaultAsync(p => p.PatientId == id);
        if (patient == null) return NotFound();

        var addr = patient.Addresses.FirstOrDefault();
        return View(new PatientFormViewModel
        {
            PatientId = patient.PatientId,
            PersonId = patient.PersonId,
            FirstName = patient.Person.FirstName,
            MiddleName = patient.Person.MiddleName,
            LastName = patient.Person.LastName,
            Gender = patient.Person.Gender,
            DateOfBirth = patient.Person.DateOfBirth,
            Phone = patient.Person.Phone,
            Email = patient.Person.Email,
            PatientCode = patient.PatientCode,
            BloodType = patient.BloodType,
            Allergies = patient.Allergies,
            MedicalHistory = patient.MedicalHistory,
            AddressId = addr?.AddressId,
            AddressLine1 = addr?.AddressLine1 ?? "",
            AddressLine2 = addr?.AddressLine2,
            City = addr?.City ?? "",
            State = addr?.State,
            PostalCode = addr?.PostalCode
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [SessionAuthorize("Admin", "Operator")]
    public async Task<IActionResult> Edit(int id, PatientFormViewModel model)
    {
        if (id != model.PatientId) return NotFound();
        if (!ModelState.IsValid) return View(model);

        var patient = await _db.Patients.Include(p => p.Person).Include(p => p.Addresses)
            .FirstOrDefaultAsync(p => p.PatientId == id);
        if (patient == null) return NotFound();

        patient.Person.FirstName = model.FirstName;
        patient.Person.MiddleName = model.MiddleName;
        patient.Person.LastName = model.LastName;
        patient.Person.Gender = model.Gender;
        patient.Person.DateOfBirth = model.DateOfBirth;
        patient.Person.Phone = model.Phone;
        patient.Person.Email = model.Email;
        patient.PatientCode = model.PatientCode;
        patient.BloodType = model.BloodType;
        patient.Allergies = model.Allergies;
        patient.MedicalHistory = model.MedicalHistory;

        var addr = patient.Addresses.FirstOrDefault();
        if (addr == null)
        {
            _db.Addresses.Add(new Address
            {
                PatientId = patient.PatientId,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                City = model.City,
                State = model.State,
                PostalCode = model.PostalCode
            });
        }
        else
        {
            addr.AddressLine1 = model.AddressLine1;
            addr.AddressLine2 = model.AddressLine2;
            addr.City = model.City;
            addr.State = model.State;
            addr.PostalCode = model.PostalCode;
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = "Patient updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [SessionAuthorize("Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var patient = await _db.Patients.FindAsync(id);
        if (patient == null) return NotFound();

        _db.Patients.Remove(patient);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Patient deleted.";
        return RedirectToAction(nameof(Index));
    }

    [SessionAuthorize("Admin", "Operator", "Nurse", "Doctor")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadDocument(int patientId, string documentName, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select a file.";
            return RedirectToAction(nameof(Details), new { id = patientId });
        }

        var path = await _files.SaveFileAsync(file, "patients");
        _db.PatientDocuments.Add(new PatientDocument
        {
            PatientId = patientId,
            DocumentName = documentName,
            FilePath = path,
            FileType = Path.GetExtension(file.FileName),
            UploadedByUserId = HttpContext.Session.GetInt32(SessionKeys.UserId)
        });
        await _db.SaveChangesAsync();
        TempData["Success"] = "Document uploaded.";
        return RedirectToAction(nameof(Details), new { id = patientId });
    }

    [SessionAuthorize("Admin", "Operator", "Nurse", "Doctor")]
    public async Task<IActionResult> DownloadDocument(int id)
    {
        var doc = await _db.PatientDocuments.FindAsync(id);
        if (doc == null) return NotFound();

        var physical = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", doc.FilePath.TrimStart('/'));
        if (!System.IO.File.Exists(physical)) return NotFound();

        return PhysicalFile(physical, "application/octet-stream", Path.GetFileName(physical));
    }
}

