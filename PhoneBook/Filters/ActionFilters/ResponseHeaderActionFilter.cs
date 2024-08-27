using Microsoft.AspNetCore.Mvc.Filters;

namespace PhoneBook.Filters.ActionFilters;

public class ResponseHeaderActionFilter : ActionFilterAttribute
{
    private readonly string _key;
    private readonly string _value;

    public ResponseHeaderActionFilter( string key, string value, int order)
    {
        this._key = key;
        this._value = value;
        Order = order;
    }
    
    public int Order { get; set; }
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await next();

        context.HttpContext.Response.Headers[_key] = _value;
    }
}