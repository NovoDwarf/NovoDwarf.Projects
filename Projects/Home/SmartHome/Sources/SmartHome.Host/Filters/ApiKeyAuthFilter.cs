using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SmartHome.Host.Filters;

/// <summary>Validates the X-Api-Key header against HealthApi:ApiKey from configuration.</summary>
public sealed class ApiKeyAuthFilter(IConfiguration cfg) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var expectedKey = cfg["HealthApi:ApiKey"];

        if (string.IsNullOrWhiteSpace(expectedKey))
        {
            context.Result = new ObjectResult("HealthApi:ApiKey is not configured.")
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue("X-Api-Key", out var provided) || provided != expectedKey)
        {
            context.Result = new UnauthorizedResult();
          
            return;
        }

        await next();
    }
}

/// <summary>Apply to a controller or action to require a valid X-Api-Key header.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ApiKeyAuthAttribute() : TypeFilterAttribute(typeof(ApiKeyAuthFilter));
