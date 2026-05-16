namespace ClinicManagementSystem.Services;

public static class SessionExtensions
{
    public static void SetUserSession(this ISession session, UserSessionInfo user)
    {
        session.SetInt32(SessionKeys.UserId, user.UserId);
        session.SetInt32(SessionKeys.PersonId, user.PersonId);
        session.SetString(SessionKeys.Username, user.Username);
        session.SetString(SessionKeys.Role, user.Role);
        session.SetString(SessionKeys.FullName, user.FullName);
        if (user.DoctorId.HasValue)
            session.SetInt32(SessionKeys.DoctorId, user.DoctorId.Value);
    }

    public static void ClearUserSession(this ISession session)
    {
        session.Clear();
    }

    public static bool IsLoggedIn(this ISession session) =>
        session.GetInt32(SessionKeys.UserId).HasValue;

    public static string? GetRole(this ISession session) =>
        session.GetString(SessionKeys.Role);

    public static bool IsInRole(this ISession session, params string[] roles)
    {
        var role = session.GetRole();
        return role != null && roles.Contains(role);
    }
}
