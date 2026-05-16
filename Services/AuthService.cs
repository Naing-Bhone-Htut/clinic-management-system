using ClinicManagementSystem.Data;
using ClinicManagementSystem.Models.Enums;
using ClinicManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ClinicManagementSystem.Services;

public class AuthService
{
    private readonly ClinicDbContext _db;

    public AuthService(ClinicDbContext db) => _db = db;

    public async Task<UserSessionInfo?> LoginAsync(LoginViewModel model)
    {
        var user = await _db.Users
            .Include(u => u.Person)
            .FirstOrDefaultAsync(u => u.Username == model.Username && u.IsActive);

        if (user == null || !PasswordHelper.Verify(model.Password, user.PasswordHash))
            return null;

        int? doctorId = null;
        if (user.Role == UserRole.Doctor)
        {
            doctorId = await _db.Doctors
                .Where(d => d.PersonId == user.PersonId)
                .Select(d => d.DoctorId)
                .FirstOrDefaultAsync();
        }

        return new UserSessionInfo
        {
            UserId = user.UserId,
            PersonId = user.PersonId,
            Username = user.Username,
            Role = user.Role.ToString(),
            FullName = user.Person.FullName,
            DoctorId = doctorId == 0 ? null : doctorId
        };
    }
}

public class UserSessionInfo
{
    public int UserId { get; set; }
    public int PersonId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int? DoctorId { get; set; }
}
