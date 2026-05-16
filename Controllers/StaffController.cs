using ClinicManagementSystem.Data;
using ClinicManagementSystem.Filters;
using ClinicManagementSystem.Models;
using ClinicManagementSystem.Models.Enums;
using ClinicManagementSystem.Services;
using ClinicManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Controllers;

[SessionAuthorize("Admin")]
public class StaffController : Controller
{
    private readonly ClinicDbContext _db;

    public StaffController(ClinicDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var staff = await _db.Users
            .Include(u => u.Person)
            .Where(u => u.Role != UserRole.Admin)
            .OrderBy(u => u.Role)
            .ThenBy(u => u.Person.LastName)
            .ToListAsync();
        return View(staff);
    }

    public IActionResult Create() => View(new StaffFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StaffFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _db.Users.AnyAsync(u => u.Username == model.Username))
        {
            ModelState.AddModelError(nameof(model.Username), "Username already exists.");
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

        switch (model.StaffType)
        {
            case "Doctor":
                _db.Doctors.Add(new Doctor
                {
                    PersonId = person.PersonId,
                    LicenseNumber = model.LicenseNumber ?? "DOC-NEW",
                    Specialty = model.Specialty,
                    Department = model.Department,
                    IsActive = model.IsActive
                });
                model.Role = UserRole.Doctor;
                break;
            case "Nurse":
                _db.Nurses.Add(new Nurse
                {
                    PersonId = person.PersonId,
                    LicenseNumber = model.LicenseNumber,
                    Department = model.Department,
                    IsActive = model.IsActive
                });
                model.Role = UserRole.Nurse;
                break;
            default:
                _db.Operators.Add(new OperatorStaff
                {
                    PersonId = person.PersonId,
                    Department = model.Department,
                    IsActive = model.IsActive
                });
                model.Role = UserRole.Operator;
                break;
        }

        if (model.CreateUserAccount && !string.IsNullOrEmpty(model.Password))
        {
            _db.Users.Add(new User
            {
                PersonId = person.PersonId,
                Username = model.Username,
                PasswordHash = PasswordHelper.Hash(model.Password),
                Role = model.Role,
                IsActive = model.IsActive
            });
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = "Staff member created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var user = await _db.Users.Include(u => u.Person).FirstOrDefaultAsync(u => u.UserId == id);
        if (user == null) return NotFound();

        var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.PersonId == user.PersonId);
        var nurse = await _db.Nurses.FirstOrDefaultAsync(n => n.PersonId == user.PersonId);

        return View(new StaffFormViewModel
        {
            UserId = user.UserId,
            PersonId = user.PersonId,
            StaffType = user.Role.ToString(),
            FirstName = user.Person.FirstName,
            MiddleName = user.Person.MiddleName,
            LastName = user.Person.LastName,
            Gender = user.Person.Gender,
            DateOfBirth = user.Person.DateOfBirth,
            Phone = user.Person.Phone,
            Email = user.Person.Email,
            Username = user.Username,
            Role = user.Role,
            LicenseNumber = doctor?.LicenseNumber ?? nurse?.LicenseNumber,
            Specialty = doctor?.Specialty,
            Department = doctor?.Department ?? nurse?.Department,
            IsActive = user.IsActive,
            CreateUserAccount = false
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StaffFormViewModel model)
    {
        if (id != model.UserId) return NotFound();
        if (!ModelState.IsValid) return View(model);

        var user = await _db.Users.Include(u => u.Person).FirstOrDefaultAsync(u => u.UserId == id);
        if (user == null) return NotFound();

        user.Person.FirstName = model.FirstName;
        user.Person.MiddleName = model.MiddleName;
        user.Person.LastName = model.LastName;
        user.Person.Gender = model.Gender;
        user.Person.DateOfBirth = model.DateOfBirth;
        user.Person.Phone = model.Phone;
        user.Person.Email = model.Email;
        user.Username = model.Username;
        user.IsActive = model.IsActive;

        if (!string.IsNullOrEmpty(model.Password))
            user.PasswordHash = PasswordHelper.Hash(model.Password);

        await _db.SaveChangesAsync();
        TempData["Success"] = "Staff updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null || user.Role == UserRole.Admin) return NotFound();

        user.IsActive = false;
        await _db.SaveChangesAsync();
        TempData["Success"] = "User deactivated.";
        return RedirectToAction(nameof(Index));
    }
}

