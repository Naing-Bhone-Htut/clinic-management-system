using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.ViewModels;

public class DashboardViewModel
{
    public int TotalPatients { get; set; }
    public int TotalDoctors { get; set; }
    public int TodaysPatients { get; set; }
    public decimal IncomeSummary { get; set; }
    public decimal ExpenseSummary { get; set; }
    public List<PatientCheckIn> RecentCheckIns { get; set; } = new();
}
