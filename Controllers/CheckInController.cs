using ClinicManagementSystem.Data;
using ClinicManagementSystem.Filters;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Services;
using ClinicManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers;

[SessionAuthorize("Admin", "Operator", "Nurse", "Doctor")]
public class CheckInController : Controller
{
    private readonly ClinicDbContext _db;

    public CheckInController(ClinicDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var query = _db.PatientCheckIns
            .Include(c => c.Patient).ThenInclude(p => p.Person)
            .Include(c => c.Doctor).ThenInclude(d => d!.Person)
            .Include(c => c.VitalSign)
            .Where(c => c.CheckInDateTime >= today);

        if (HttpContext.Session.GetRole() == "Doctor")
        {
            var doctorId = HttpContext.Session.GetInt32(SessionKeys.DoctorId);
            if (doctorId.HasValue)
                query = query.Where(c => c.DoctorId == doctorId);
        }

        var list = await query.OrderByDescending(c => c.CheckInDateTime).ToListAsync();
        return View(list);
    }



    [SessionAuthorize("Admin", "Operator", "Nurse")]
    public async Task<IActionResult> Create()
    {
        return View(await BuildFormAsync(new CheckInFormViewModel()));
    }


    [HttpPost, ValidateAntiForgeryToken]
    [SessionAuthorize("Admin", "Operator", "Nurse")]
    public async Task<IActionResult> Create(CheckInFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(await BuildFormAsync(model));

        var checkIn = new PatientCheckIn
        {
            PatientId = model.PatientId,
            DoctorId = model.DoctorId,
            CheckInDateTime = model.CheckInDateTime,
            IsServed = model.IsServed,
            Notes = model.Notes,
            CreatedByUserId = HttpContext.Session.GetInt32(SessionKeys.UserId)
        };
        _db.PatientCheckIns.Add(checkIn);
        await _db.SaveChangesAsync();

        if (model.Temperature.HasValue || model.BloodPressure != null)
        {
            _db.VitalSigns.Add(new VitalSign
            {
                CheckInId = checkIn.CheckInId,
                BloodPressure = model.BloodPressure,
                Temperature = model.Temperature,
                PulseRate = model.PulseRate,
                RespiratoryRate = model.RespiratoryRate,
                Weight = model.Weight,
                Height = model.Height,
                RecordedByUserId = HttpContext.Session.GetInt32(SessionKeys.UserId)
            });
            await _db.SaveChangesAsync();
        }

        TempData["Success"] = "Patient checked in.";
        return RedirectToAction(nameof(Index));
    }



    public async Task<IActionResult> Edit(int id)
    {
        var checkIn = await _db.PatientCheckIns.Include(c => c.VitalSign).FirstOrDefaultAsync(c => c.CheckInId == id);
        if (checkIn == null) return NotFound();

        var model = new CheckInFormViewModel
        {
            CheckInId = checkIn.CheckInId,
            PatientId = checkIn.PatientId,
            DoctorId = checkIn.DoctorId,
            CheckInDateTime = checkIn.CheckInDateTime,
            CheckOutDateTime = checkIn.CheckOutDateTime,
            IsServed = checkIn.IsServed,
            Notes = checkIn.Notes,
            BloodPressure = checkIn.VitalSign?.BloodPressure,
            Temperature = checkIn.VitalSign?.Temperature,
            PulseRate = checkIn.VitalSign?.PulseRate,
            RespiratoryRate = checkIn.VitalSign?.RespiratoryRate,
            Weight = checkIn.VitalSign?.Weight,
            Height = checkIn.VitalSign?.Height
        };
        return View(await BuildFormAsync(model));
    }



    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CheckInFormViewModel model)
    {
        if (id != model.CheckInId) return NotFound();
        if (!ModelState.IsValid)
            return View(await BuildFormAsync(model));

        var checkIn = await _db.PatientCheckIns.Include(c => c.VitalSign).FirstOrDefaultAsync(c => c.CheckInId == id);
        if (checkIn == null) return NotFound();

        checkIn.DoctorId = model.DoctorId;
        checkIn.CheckOutDateTime = model.CheckOutDateTime;
        checkIn.IsServed = model.IsServed;
        checkIn.Notes = model.Notes;

        if (HttpContext.Session.IsInRole("Admin", "Doctor"))
            checkIn.DoctorId = model.DoctorId;

        if (checkIn.VitalSign == null)
        {
            _db.VitalSigns.Add(new VitalSign
            {
                CheckInId = checkIn.CheckInId,
                BloodPressure = model.BloodPressure,
                Temperature = model.Temperature,
                PulseRate = model.PulseRate,
                RespiratoryRate = model.RespiratoryRate,
                Weight = model.Weight,
                Height = model.Height,
                RecordedByUserId = HttpContext.Session.GetInt32(SessionKeys.UserId)
            });
        }
        else
        {
            checkIn.VitalSign.BloodPressure = model.BloodPressure;
            checkIn.VitalSign.Temperature = model.Temperature;
            checkIn.VitalSign.PulseRate = model.PulseRate;
            checkIn.VitalSign.RespiratoryRate = model.RespiratoryRate;
            checkIn.VitalSign.Weight = model.Weight;
            checkIn.VitalSign.Height = model.Height;
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = "Check-in updated.";
        return RedirectToAction(nameof(Index));
    }


    [HttpPost, ValidateAntiForgeryToken]
    [SessionAuthorize("Admin", "Operator", "Nurse")]
    public async Task<IActionResult> CheckOut(int id)
    {
        var checkIn = await _db.PatientCheckIns.FindAsync(id);
        if (checkIn == null) return NotFound();

        checkIn.CheckOutDateTime = DateTime.Now;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Patient checked out.";
        return RedirectToAction(nameof(Index));
    }


    private async Task<CheckInFormViewModel> BuildFormAsync(CheckInFormViewModel model)
    {
        model.Patients = await _db.Patients
            .Include(p => p.Person)
            .Select(p => new SelectListItem($"{p.PatientCode} - {p.Person.FirstName} {p.Person.LastName}", p.PatientId.ToString(), p.PatientId == model.PatientId))
            .ToListAsync();

        model.Doctors = await _db.Doctors
            .Include(d => d.Person)
            .Where(d => d.IsActive)
            .Select(d => new SelectListItem(d.Person.FullName, d.DoctorId.ToString(), d.DoctorId == model.DoctorId))
            .ToListAsync();

        return model;
    }
}

