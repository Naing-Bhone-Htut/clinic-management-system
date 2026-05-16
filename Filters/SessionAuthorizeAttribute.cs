using ClinicManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClinicManagementSystem.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class SessionAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public string[] Roles { get; }

    public SessionAuthorizeAttribute(params string[] roles)
    {
        Roles = roles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var session = context.HttpContext.Session;

        if (!session.IsLoggedIn())
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        if (Roles.Length > 0 && !session.IsInRole(Roles))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
        }
    }
}
