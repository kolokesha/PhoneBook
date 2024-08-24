using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PhoneBook.Filters.ActionFilters.AuthorizationFilters;

public class TokenAuthorizationFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.Request.Cookies.ContainsKey("Auth-Key") == false)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
        }
        
        if (context.HttpContext.Request.Cookies["Auth-Key"] != "A100")
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
        }
        
        if (context.HttpContext.Request.Cookies["Auth-Key"] != "A100")
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
        }
    }
}