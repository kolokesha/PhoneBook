using Microsoft.AspNetCore.Mvc.Filters;

namespace PhoneBook.Filters.ActionFilters;

public class ResponseHeaderActionFilter : IAsyncActionFilter, IOrderedFilter
{

    private readonly ILogger<ResponseHeaderActionFilter> _logger;
    private readonly string _key;
    private readonly string _value;

    public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger, string key, string value, int order)
    {
        _logger = logger;
        this._key = key;
        this._value = value;
        Order = order;
    }
    
    public int Order { get; set; }
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        _logger.LogInformation("{FilterName}.{MethodName}", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
        
        await next();
        
        _logger.LogInformation("{FilterName}.{MethodName}", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

        context.HttpContext.Response.Headers[_key] = _value;
    }
}