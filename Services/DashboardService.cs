using ClinicManagementSystem.Data;
using ClinicManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Services;

public class DashboardService
{
    private readonly ClinicDbContext _db;

    public DashboardService(ClinicDbContext db) => _db = db;

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        return new DashboardViewModel
        {
            TotalPatients = await _db.Patients.CountAsync(),
            TotalDoctors = await _db.Doctors.CountAsync(d => d.IsActive),
            TodaysPatients = await _db.PatientCheckIns
                .Where(c => c.CheckInDateTime >= today && c.CheckInDateTime < tomorrow)
                .CountAsync(),
            IncomeSummary = await _db.IncomeBills.SumAsync(b => b.TotalAmount),
            ExpenseSummary = await _db.ExpenseBills.SumAsync(b => b.TotalAmount),
            RecentCheckIns = await _db.PatientCheckIns
                .Include(c => c.Patient).ThenInclude(p => p.Person)
                .Include(c => c.Doctor).ThenInclude(d => d!.Person)
                .OrderByDescending(c => c.CheckInDateTime)
                .Take(5)
                .ToListAsync()
        };
    }
}
