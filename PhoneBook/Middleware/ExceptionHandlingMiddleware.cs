using Serilog;

namespace PhoneBook.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IDiagnosticContext _diagnosticContext;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IDiagnosticContext diagnosticContext)
    {
        _next = next;
        _logger = logger;
        _diagnosticContext = diagnosticContext;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception e)
        {
            if (e.InnerException != null)
            {
                _logger.LogError("{ExceptionType}, {ExceptionMessage}", e.InnerException.GetType().ToString(), e.InnerException.Message);
            }
            else
            {
                _logger.LogError("{ExceptionType}, {ExceptionMessage}", e.GetType().ToString(), e.Message);
            }

            /*httpContext.Response.StatusCode = 500;
            await httpContext.Response.WriteAsync("Error occur");*/

            throw;
        }
    }
}