using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SmartHome.Host.Filters;

/// <summary>
/// Validates the Telegram Mini App initData passed in the X-Telegram-Init-Data header.
/// Algorithm: https://core.telegram.org/bots/webapps#validating-data-received-via-the-mini-app
/// </summary>
public sealed class TelegramInitDataFilter : IAsyncActionFilter
{
    private readonly IConfiguration _cfg;
    private readonly IWebHostEnvironment _env;

    /// <summary>
    /// Validates the Telegram Mini App initData passed in the X-Telegram-Init-Data header.
    /// Algorithm: https://core.telegram.org/bots/webapps#validating-data-received-via-the-mini-app
    /// </summary>
    public TelegramInitDataFilter(IConfiguration cfg, IWebHostEnvironment env)
    {
        _cfg = cfg;
        _env = env;
    }

    // Allow up to 1 hour for the initData to be valid.
    private static readonly TimeSpan MaxAge = TimeSpan.FromHours(1);

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // In Development, skip validation to make testing easier (e.g. via Swagger/curl).
        if (_env.IsDevelopment())
        {
            await next();
            return;
        }

        var botToken = _cfg["Telegram:Token"];
        if (string.IsNullOrEmpty(botToken))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var initData = context.HttpContext.Request.Headers["X-Telegram-Init-Data"].ToString();
        if (!IsValid(initData, botToken, out var reason))
        {
            context.Result = new UnauthorizedObjectResult(new { error = reason });
            return;
        }

        await next();
    }

    private static bool IsValid(string initData, string botToken, out string reason)
    {
        reason = string.Empty;

        if (string.IsNullOrWhiteSpace(initData))
        {
            reason = "initData is missing";
            return false;
        }

        // Parse all key=value pairs from the URL-encoded initData
        var data = initData
            .Split('&')
            .Select(p => p.Split('=', 2))
            .Where(p => p.Length == 2)
            .ToDictionary(
                p => Uri.UnescapeDataString(p[0]),
                p => Uri.UnescapeDataString(p[1]));

        if (!data.TryGetValue("hash", out var receivedHash))
        {
            reason = "hash is missing";
            return false;
        }

        // Optionally check auth_date freshness
        if (data.TryGetValue("auth_date", out var authDateStr)
            && long.TryParse(authDateStr, out var authDateUnix))
        {
            var authDate = DateTimeOffset.FromUnixTimeSeconds(authDateUnix);
            if (DateTimeOffset.UtcNow - authDate > MaxAge)
            {
                reason = "initData expired";
                return false;
            }
        }

        // Build data-check-string: sorted key=value pairs joined by \n (excluding hash)
        var checkString = data
            .Where(kv => kv.Key != "hash")
            .OrderBy(kv => kv.Key)
            .Select(kv => $"{kv.Key}={kv.Value}")
            .Aggregate((a, b) => $"{a}\n{b}");

        // secret_key = HMAC-SHA256("WebAppData", bot_token)
        var secretKey = HMACSHA256.HashData(
            Encoding.UTF8.GetBytes("WebAppData"),
            Encoding.UTF8.GetBytes(botToken));

        // computed_hash = HMAC-SHA256(data-check-string, secret_key)
        var computedHash = HMACSHA256.HashData(
            secretKey,
            Encoding.UTF8.GetBytes(checkString));

        var computedHashHex = Convert.ToHexString(computedHash);

        if (computedHashHex.Equals(receivedHash, StringComparison.OrdinalIgnoreCase)) 
            return true;
        
        reason = "hash mismatch";
        return false;

    }
}

/// <summary>Apply to a controller or action to require a valid Telegram initData header.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class TelegramInitDataAttribute() : TypeFilterAttribute(typeof(TelegramInitDataFilter));
