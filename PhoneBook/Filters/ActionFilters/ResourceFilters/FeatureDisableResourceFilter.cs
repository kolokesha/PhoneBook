using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PhoneBook.Filters.ActionFilters.ResourceFilters;

public class FeatureDisableResourceFilter : IAsyncResourceFilter
{
    private readonly ILogger<FeatureDisableResourceFilter> _logger;
    private readonly bool _isDisabled;

    public FeatureDisableResourceFilter(ILogger<FeatureDisableResourceFilter> logger, bool isDisabled = true)
    {
        _logger = logger;
        _isDisabled = isDisabled;
    }

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        _logger.LogInformation("{FilterName}.{FilterMethod} before ", nameof(FeatureDisableResourceFilter), nameof(OnResourceExecutionAsync));

        if (_isDisabled)
        {
            context.Result = new StatusCodeResult(501);
        }
        else
        {
            await next();
        }
        
        _logger.LogInformation("{FilterName}.{FilterMethod} before ", nameof(FeatureDisableResourceFilter), nameof(OnResourceExecutionAsync));
    }
}