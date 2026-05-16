using ClinicManagementSystem.Filters;
using ClinicManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers;

[SessionAuthorize]
public class HomeController : Controller
{
    private readonly DashboardService _dashboard;

    public HomeController(DashboardService dashboard) => _dashboard = dashboard;

    public async Task<IActionResult> Index()
    {
        var model = await _dashboard.GetDashboardAsync();
        return View(model);
    }

    public IActionResult Error() => View();
}

